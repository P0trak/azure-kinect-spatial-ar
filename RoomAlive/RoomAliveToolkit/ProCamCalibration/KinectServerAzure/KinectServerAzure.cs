using System;
using Microsoft.Azure.Kinect.Sensor;
using RoomAliveToolkit;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Discovery;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using SharpDX.WIC;
using RoomAliveToolKit;

namespace RoomAliveToolKit
{
    public class KinectHandlerAzure:IDisposable
    {
        public static KinectHandlerAzure instance;
        Device kinectSensor;

        //depth stuff
        public ushort[] depthShortBuffer = new ushort[KinectAzureCalibration.depthImageWidth * KinectAzureCalibration.depthImageHeight];
        public byte[] depthByteBuffer = new byte[KinectAzureCalibration.depthImageWidth * KinectAzureCalibration.depthImageHeight * 2];
        public List<AutoResetEvent> depthFrameReady = new List<AutoResetEvent>();

        //colour stuff
        public byte[] yuvByteBuffer = new byte[KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 2];
        public List<AutoResetEvent> yuvFrameReady = new List<AutoResetEvent>();
        public byte[] rgbByteBuffer = new byte[KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 4];
        public List<AutoResetEvent> rgbFrameReady = new List<AutoResetEvent>();
        public byte[] jpegByteBuffer = new byte[KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 4];
        public List<AutoResetEvent> jpegFrameReady = new List<AutoResetEvent>();
        public int nJpegBytes = 0;

        public float lastColorGain;
        public long lastColorExposureTimeTicks;

        public KinectAzureCalibration kinectAzureCalibration;
        public ManualResetEvent kinectAzureCalibrationReady = new ManualResetEvent(false);

        ImagingFactory imagingFactory = new ImagingFactory();
        Stopwatch stopWatch = new Stopwatch();

        CancellationTokenSource _cancellationTokenSource;
        CancellationToken _token;

        public KinectHandlerAzure()
        {
            instance = this;
            kinectSensor = Device.Open();

            kinectAzureCalibration = new KinectAzureCalibration();
            kinectAzureCalibration.RecoverCalibrationFromSensor(kinectSensor);
            kinectAzureCalibrationReady.Set();
            //kinectSensor.CoordinateMapper.CoordinateMappingChanged += CoordinateMapper_CoordinateMappingChanged;

            _cancellationTokenSource = new CancellationTokenSource();
            _token = _cancellationTokenSource.Token;
            Task.Run(() => RunCameraThreadAsync(_token));
        }

        //not sure about the coordinate mapper stuff - do i actually need this?
        // there's also a whole bunch of what seem to be function overrides? like idk how important they are

        void processDepthFrame(Image depthFrame)
        {
            if (depthFrame != null)
            {
                using (depthFrame)
                {
                    if (depthFrameReady.Count > 0)
                    {
                        lock (depthShortBuffer)
                        {
                            depthFrame.Reference();
                            Memory<ushort> pixels = depthFrame.GetPixels<ushort>();
                            pixels.ToArray().CopyTo(depthShortBuffer, 0);
                            //depthFrame.CopyFrameDataToArray(depthShortBuffer);
                        }
                            
                        lock (depthFrameReady)
                            foreach (var autoResetEvent in depthFrameReady)
                                autoResetEvent.Set();
                    }
                }
            }
        }

        void processColourFrame(Image colourFrame) 
        {
            if (colourFrame != null)
            {
                using (colourFrame)
                {
                    lastColorGain = (float) ColorControlCommand.Gain;
                    lastColorExposureTimeTicks = colourFrame.SystemTimestampNsec / 100; //1 tick is 100 nanoseconds

                    if (yuvFrameReady.Count > 0)
                    {
                        lock (yuvByteBuffer)
                        {
                            colourFrame.Reference();
                            Memory<ushort> pixels = colourFrame.GetPixels<ushort>();
                            pixels.ToArray().CopyTo(yuvByteBuffer, 0);
                        }
                        lock (yuvFrameReady)
                            foreach (var autoResetEvent in yuvFrameReady)
                                autoResetEvent.Set();
                    }

                    if ((rgbFrameReady.Count > 0) || (jpegFrameReady.Count > 0))
                    {
                        lock (rgbByteBuffer)
                        {
                            Memory<ushort> pixels = colourFrame.GetPixels<ushort>();
                            pixels.ToArray().CopyTo(yuvByteBuffer, 0);
                        }
                            //colourFrame.CopyConvertedFrameDataToArray(rgbByteBuffer, ImageFormat.ColorBGRA32);
                            // may need to figure out how to do this
                        lock (rgbFrameReady)
                            foreach (var autoResetEvent in rgbFrameReady)
                                autoResetEvent.Set();
                    }

                    if (jpegFrameReady.Count > 0)
                    {
                        // should be put in a separate thread?

                        stopWatch.Restart();

                        var bitmapSource = new Bitmap(imagingFactory, KinectAzureCalibration.colorImageWidth, KinectAzureCalibration.colorImageHeight, SharpDX.WIC.PixelFormat.Format32bppBGR, BitmapCreateCacheOption.CacheOnLoad);
                        var bitmapLock = bitmapSource.Lock(BitmapLockFlags.Write);
                        Marshal.Copy(rgbByteBuffer, 0, bitmapLock.Data.DataPointer, KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 4);
                        bitmapLock.Dispose();

                        var memoryStream = new MemoryStream();

                        //var fileStream = new FileStream("test" + frame++ + ".jpg", FileMode.Create);
                        //var stream = new WICStream(imagingFactory, "test" + frame++ + ".jpg", SharpDX.IO.NativeFileAccess.Write);

                        var stream = new WICStream(imagingFactory, memoryStream);

                        var jpegBitmapEncoder = new JpegBitmapEncoder(imagingFactory);
                        jpegBitmapEncoder.Initialize(stream);

                        var bitmapFrameEncode = new BitmapFrameEncode(jpegBitmapEncoder);
                        bitmapFrameEncode.Options.ImageQuality = 0.5f;
                        bitmapFrameEncode.Initialize();
                        bitmapFrameEncode.SetSize(KinectAzureCalibration.colorImageWidth, KinectAzureCalibration.colorImageHeight);
                        var pixelFormatGuid = PixelFormat.FormatDontCare;
                        bitmapFrameEncode.SetPixelFormat(ref pixelFormatGuid);
                        bitmapFrameEncode.WriteSource(bitmapSource);

                        bitmapFrameEncode.Commit();
                        jpegBitmapEncoder.Commit();

                        //fileStream.Close();
                        //fileStream.Dispose();

                        //Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms " + memoryStream.Length + " bytes");

                        lock (jpegByteBuffer)
                        {
                            nJpegBytes = (int)memoryStream.Length;
                            memoryStream.Seek(0, SeekOrigin.Begin);
                            memoryStream.Read(jpegByteBuffer, 0, nJpegBytes);
                        }
                        lock (jpegFrameReady)
                            foreach (var autoResetEvent in jpegFrameReady)
                                autoResetEvent.Set();

                        //var file = new FileStream("test" + frame++ + ".jpg", FileMode.Create);
                        //file.Write(jpegByteBuffer, 0, nJpegBytes);
                        //file.Close();

                        bitmapSource.Dispose();
                        memoryStream.Close();
                        memoryStream.Dispose();
                        stream.Dispose();
                        jpegBitmapEncoder.Dispose();
                        bitmapFrameEncode.Dispose();
                    }
                }
            }
        }

        void RunCameraThreadAsync(CancellationToken token)
        {
            kinectSensor.StartCameras(new DeviceConfiguration()
            {
                CameraFPS = FPS.FPS30,
                ColorResolution = ColorResolution.Off,
                DepthMode = DepthMode.NFOV_Unbinned,
                WiredSyncMode = WiredSyncMode.Standalone,
            });

            while(!token.IsCancellationRequested)
            {
                Capture capture = kinectSensor.GetCapture();

                if (capture != null)
                {
                    Image colour = capture.Color;
                    Image depth = capture.Depth;

                    processColourFrame(colour);
                    processDepthFrame(depth);
                }
            }
            Dispose();
        }

        public void Dispose()
        {
           kinectSensor.Dispose();
        }

        //idk if we need any of the body frame stuff

    }

    /// <summary>
    /// Created on each session.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
    [ServiceContract]
    public class KinectServerAzure
    {
        byte[] depthByteBuffer = new byte[KinectAzureCalibration.depthImageWidth * KinectAzureCalibration.depthImageHeight * 2];
        byte[] yuvByteBuffer = new byte[KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 2];
        byte[] rgbByteBuffer = new byte[KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 4];

        AutoResetEvent depthFrameReady = new AutoResetEvent(false);
        AutoResetEvent yuvFrameReady = new AutoResetEvent(false);
        AutoResetEvent rgbFrameReady = new AutoResetEvent(false);
        AutoResetEvent jpegFrameReady = new AutoResetEvent(false);
        //AutoResetEvent audioFrameReady = new AutoResetEvent(false);

        //Queue<byte[]> audioFrameQueue = new Queue<byte[]>();

        public KinectServerAzure()
        {
            lock (KinectHandlerAzure.instance.depthFrameReady) // overkill?
                KinectHandlerAzure.instance.depthFrameReady.Add(depthFrameReady);
            lock (KinectHandlerAzure.instance.yuvFrameReady)
                KinectHandlerAzure.instance.yuvFrameReady.Add(yuvFrameReady);
            lock (KinectHandlerAzure.instance.rgbFrameReady)
                KinectHandlerAzure.instance.rgbFrameReady.Add(rgbFrameReady);
            lock (KinectHandlerAzure.instance.jpegFrameReady)
                KinectHandlerAzure.instance.jpegFrameReady.Add(jpegFrameReady);
            /*
            lock (KinectHandlerAzure.instance.audioFrameReady)
                KinectHandlerAzure.instance.audioFrameReady.Add(audioFrameReady);
            lock (KinectHandlerAzure.instance.audioFrameQueues)
                KinectHandlerAzure.instance.audioFrameQueues.Add(audioFrameQueue);
            */


            OperationContext.Current.Channel.Closed += ClientClosed;
        }

        public void ClientClosed(object sender, EventArgs e)
        {
            //Console.WriteLine("ClientClosed");

            // remove ourselves from the singleton
            lock (KinectHandlerAzure.instance.depthFrameReady) // overkill?
                KinectHandlerAzure.instance.depthFrameReady.Remove(depthFrameReady);
            lock (KinectHandlerAzure.instance.yuvFrameReady)
                KinectHandlerAzure.instance.yuvFrameReady.Remove(yuvFrameReady);
            lock (KinectHandlerAzure.instance.rgbFrameReady)
                KinectHandlerAzure.instance.rgbFrameReady.Remove(rgbFrameReady);
            lock (KinectHandlerAzure.instance.jpegFrameReady)
                KinectHandlerAzure.instance.jpegFrameReady.Remove(jpegFrameReady);
            /*
            lock (KinectHandlerAzure.instance.audioFrameReady)
                KinectHandlerAzure.instance.audioFrameReady.Remove(audioFrameReady);
            lock (KinectHandlerAzure.instance.audioFrameQueues)
                KinectHandlerAzure.instance.audioFrameQueues.Remove(audioFrameQueue);
            */
        }

        // Returns immediately if a frame has been made available since the last time this was called on this client;
        // otherwise blocks until one is available.
        [OperationContract]
        public byte[] LatestDepthImage()
        {
            depthFrameReady.WaitOne();
            // Is this copy really necessary?:
            lock (KinectHandlerAzure.instance.depthShortBuffer)
                Buffer.BlockCopy(KinectHandlerAzure.instance.depthShortBuffer, 0, depthByteBuffer, 0, KinectAzureCalibration.depthImageWidth * KinectAzureCalibration.depthImageHeight * 2);
            return depthByteBuffer;
        }

        [OperationContract]
        public byte[] LatestYUVImage()
        {
            yuvFrameReady.WaitOne();
            lock (KinectHandlerAzure.instance.yuvByteBuffer)
                Buffer.BlockCopy(KinectHandlerAzure.instance.yuvByteBuffer, 0, yuvByteBuffer, 0, KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 2);
            return yuvByteBuffer;
        }

        [OperationContract]
        public byte[] LatestRGBImage()
        {
            rgbFrameReady.WaitOne();
            lock (KinectHandlerAzure.instance.rgbByteBuffer)
                Buffer.BlockCopy(KinectHandlerAzure.instance.rgbByteBuffer, 0, rgbByteBuffer, 0, KinectAzureCalibration.colorImageWidth * KinectAzureCalibration.colorImageHeight * 4);
            return rgbByteBuffer;
        }

        [OperationContract]
        public byte[] LatestJPEGImage()
        {
            jpegFrameReady.WaitOne();
            byte[] jpegByteBuffer;
            lock (KinectHandlerAzure.instance.jpegByteBuffer)
            {
                jpegByteBuffer = new byte[KinectHandlerAzure.instance.nJpegBytes];
                Buffer.BlockCopy(KinectHandlerAzure.instance.jpegByteBuffer, 0, jpegByteBuffer, 0, KinectHandlerAzure.instance.nJpegBytes);
            }
            return jpegByteBuffer;
        }

        /*
        [OperationContract]
        public byte[] LatestAudio()
        {
            audioFrameReady.WaitOne();
            lock (KinectHandler.instance.audioFrameQueues) // overkill?
            {
                var buffer = new byte[audioFrameQueue.Count * 1024];
                int count = audioFrameQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    var thisBuffer = audioFrameQueue.Dequeue();
                    Array.Copy(thisBuffer, 0, buffer, 1024 * i, 1024);
                }
                return buffer;
            }
        }
        */

        [OperationContract]
        public float LastColorGain()
        {
            return KinectHandlerAzure.instance.lastColorGain;
        }

        [OperationContract]
        public long LastColorExposureTimeTicks()
        {
            return KinectHandlerAzure.instance.lastColorExposureTimeTicks;
        }
        [OperationContract]
        public KinectAzureCalibration GetCalibration()
        {
            KinectHandlerAzure.instance.kinectAzureCalibrationReady.WaitOne();
            return KinectHandlerAzure.instance.kinectAzureCalibration;
        }
    }
}
class Program
{
    static void Main(string[] args)
    {
        new KinectHandlerAzure();
        var serviceHost = new ServiceHost(typeof(KinectServerAzure));

        // discovery
        serviceHost.Description.Behaviors.Add(new ServiceDiscoveryBehavior());
        serviceHost.AddServiceEndpoint(new UdpDiscoveryEndpoint());

        Console.WriteLine("hi :)");
        serviceHost.Open();
        Console.ReadLine();
    }
}
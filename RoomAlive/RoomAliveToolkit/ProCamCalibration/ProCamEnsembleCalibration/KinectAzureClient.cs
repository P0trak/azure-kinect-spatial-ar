﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------



[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
[System.ServiceModel.ServiceContractAttribute(ConfigurationName="KinectServerAzure")]
public interface KinectServerAzure
{
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestDepthImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestDepthImageResponse")]
    byte[] LatestDepthImage();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestDepthImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestDepthImageResponse")]
    System.Threading.Tasks.Task<byte[]> LatestDepthImageAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestYUVImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestYUVImageResponse")]
    byte[] LatestYUVImage();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestYUVImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestYUVImageResponse")]
    System.Threading.Tasks.Task<byte[]> LatestYUVImageAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestRGBImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestRGBImageResponse")]
    byte[] LatestRGBImage();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestRGBImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestRGBImageResponse")]
    System.Threading.Tasks.Task<byte[]> LatestRGBImageAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestJPEGImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestJPEGImageResponse")]
    byte[] LatestJPEGImage();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LatestJPEGImage", ReplyAction="http://tempuri.org/KinectServerAzure/LatestJPEGImageResponse")]
    System.Threading.Tasks.Task<byte[]> LatestJPEGImageAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LastColorGain", ReplyAction="http://tempuri.org/KinectServerAzure/LastColorGainResponse")]
    float LastColorGain();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LastColorGain", ReplyAction="http://tempuri.org/KinectServerAzure/LastColorGainResponse")]
    System.Threading.Tasks.Task<float> LastColorGainAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LastColorExposureTimeTicks", ReplyAction="http://tempuri.org/KinectServerAzure/LastColorExposureTimeTicksResponse")]
    long LastColorExposureTimeTicks();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/LastColorExposureTimeTicks", ReplyAction="http://tempuri.org/KinectServerAzure/LastColorExposureTimeTicksResponse")]
    System.Threading.Tasks.Task<long> LastColorExposureTimeTicksAsync();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/GetCalibration", ReplyAction="http://tempuri.org/KinectServerAzure/GetCalibrationResponse")]
    RoomAliveToolkit.Kinect2Calibration GetCalibration();
    
    [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/KinectServerAzure/GetCalibration", ReplyAction="http://tempuri.org/KinectServerAzure/GetCalibrationResponse")]
    System.Threading.Tasks.Task<RoomAliveToolkit.Kinect2Calibration> GetCalibrationAsync();
}

[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public interface KinectServerAzureChannel : KinectServerAzure, System.ServiceModel.IClientChannel
{
}

[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
public partial class KinectServerAzureClient : System.ServiceModel.ClientBase<KinectServerAzure>, KinectServerAzure
{
    
    public KinectServerAzureClient()
    {
    }
    
    public KinectServerAzureClient(string endpointConfigurationName) : 
            base(endpointConfigurationName)
    {
    }
    
    public KinectServerAzureClient(string endpointConfigurationName, string remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public KinectServerAzureClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(endpointConfigurationName, remoteAddress)
    {
    }
    
    public KinectServerAzureClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
            base(binding, remoteAddress)
    {
    }
    
    public byte[] LatestDepthImage()
    {
        return base.Channel.LatestDepthImage();
    }
    
    public System.Threading.Tasks.Task<byte[]> LatestDepthImageAsync()
    {
        return base.Channel.LatestDepthImageAsync();
    }
    
    public byte[] LatestYUVImage()
    {
        return base.Channel.LatestYUVImage();
    }
    
    public System.Threading.Tasks.Task<byte[]> LatestYUVImageAsync()
    {
        return base.Channel.LatestYUVImageAsync();
    }
    
    public byte[] LatestRGBImage()
    {
        return base.Channel.LatestRGBImage();
    }
    
    public System.Threading.Tasks.Task<byte[]> LatestRGBImageAsync()
    {
        return base.Channel.LatestRGBImageAsync();
    }
    
    public byte[] LatestJPEGImage()
    {
        return base.Channel.LatestJPEGImage();
    }
    
    public System.Threading.Tasks.Task<byte[]> LatestJPEGImageAsync()
    {
        return base.Channel.LatestJPEGImageAsync();
    }
    
    public float LastColorGain()
    {
        return base.Channel.LastColorGain();
    }
    
    public System.Threading.Tasks.Task<float> LastColorGainAsync()
    {
        return base.Channel.LastColorGainAsync();
    }
    
    public long LastColorExposureTimeTicks()
    {
        return base.Channel.LastColorExposureTimeTicks();
    }
    
    public System.Threading.Tasks.Task<long> LastColorExposureTimeTicksAsync()
    {
        return base.Channel.LastColorExposureTimeTicksAsync();
    }
    
    public RoomAliveToolkit.Kinect2Calibration GetCalibration()
    {
        return base.Channel.GetCalibration();
    }
    
    public System.Threading.Tasks.Task<RoomAliveToolkit.Kinect2Calibration> GetCalibrationAsync()
    {
        return base.Channel.GetCalibrationAsync();
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POSTest.IndigoWebService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://schemas.veneka.com/ISO8583", ConfigurationName="IndigoWebService.IISO8583")]
    public interface IISO8583 {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://schemas.veneka.com/ISO8583/IISO8583/Send", ReplyAction="http://schemas.veneka.com/ISO8583/IISO8583/SendResponse")]
        Veneka.Module.ISO8583.WCF.ISOMessage Send(Veneka.Module.ISO8583.WCF.ISOMessage message);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IISO8583Channel : POSTest.IndigoWebService.IISO8583, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ISO8583Client : System.ServiceModel.ClientBase<POSTest.IndigoWebService.IISO8583>, POSTest.IndigoWebService.IISO8583 {
        
        public ISO8583Client() {
        }
        
        public ISO8583Client(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ISO8583Client(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ISO8583Client(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ISO8583Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public Veneka.Module.ISO8583.WCF.ISOMessage Send(Veneka.Module.ISO8583.WCF.ISOMessage message) {
            return base.Channel.Send(message);
        }
    }
}

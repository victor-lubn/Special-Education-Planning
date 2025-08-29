using System;
using System.ServiceModel;
using System.ServiceModel.Channels;



namespace SpecialEducationPlanning
.Api.ConnectedServices.SAP
{



    public partial class MI_1519_DV_SAPC_Request_Intl_OutClient
    {



        /// <summary>
        /// </summary>
        /// <param name="endpointConfiguration"></param>
        /// <param name="remoteAddress"></param>
        /// <param name="timeout"></param>
        public MI_1519_DV_SAPC_Request_Intl_OutClient(EndpointConfiguration endpointConfiguration,
            EndpointAddress remoteAddress,
            TimeSpan timeout) :
            base(GetBindingForEndpointExtension(endpointConfiguration, timeout), remoteAddress)
        {
            Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(Endpoint, ClientCredentials);
        }



        #region Methods Private



        private static Binding GetBindingForEndpointExtension(EndpointConfiguration endpointConfiguration,
            TimeSpan timeout)
        {
            var result = GetBindingForEndpoint(endpointConfiguration);
            result.CloseTimeout = timeout;
            result.OpenTimeout = timeout;
            result.ReceiveTimeout = timeout;
            result.SendTimeout = timeout;



            return result;
        }



        #endregion



    }



}
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace SFA.DAS.NServiceBus.AzureFunctions.ServiceBus
{
    [Serializable]
    public class EndpointQueueAttributeNotFoundException : Exception
    {
        public EndpointQueueAttributeNotFoundException(Assembly assemblyWithTriggerAttribute)
            : base($"No [NServiceBusTriggerFunctionAttribute] attribute was found in { assemblyWithTriggerAttribute.FullName}")
        {
        }

        public EndpointQueueAttributeNotFoundException(string? message) : base(message)
        {
        }

        public EndpointQueueAttributeNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected EndpointQueueAttributeNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
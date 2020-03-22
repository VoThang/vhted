using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VHTED.Api.Models.IosMobile
{
    public class GetTwilioCredentialRequest
    {
        public Guid CallTransactionId { get; set; }
    }

    public class GetTwilioCredentialResponse
    {
        public string RoomId { get; set; }
        public string PatientToken { get; set; }
    }
}

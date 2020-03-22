using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using VHTED.Api.Model;
using VHTED.Api.Model.IosModel;
using VHTED.Api.Service.Service;

namespace VHTED.Api.Controllers
{
    [Authorize]
    [ApiController]
    public class IosPatientController : ControllerBase
    {
        private readonly ICallTransactionService _callTransactionService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IPatientService _patientService;
        private readonly IPatientMedicationHistoryService _medicationHistoryService;

        public IosPatientController(ICallTransactionService callTransactionService, IHostingEnvironment hostingEnvironment, IPatientService patientService,
            IPatientMedicationHistoryService medicationHistoryService)
        {
            _callTransactionService = callTransactionService;
            _hostingEnvironment = hostingEnvironment;
            _patientService = patientService;
            _medicationHistoryService = medicationHistoryService;
        }

        [Route("api/payment_receipt")]
        [HttpGet]
        public async Task<WebServiceResult> GetPaymentReceipt()
        {
            var patientId = await GetCurrentPatientId();

            if (patientId > 0)
            {
                var paymentReceipts = _callTransactionService.IosGetVisitHistory(patientId);

                decimal totalPayment = 0;
                if (paymentReceipts != null)
                {
                    foreach (var payment in paymentReceipts)
                    {
                        totalPayment += payment.Payment;
                    }
                }

                var response = new PaymentReceiptResponseModel()
                {
                    TotalPayment = string.Format("${0:0.00}", totalPayment),
                    Payments = paymentReceipts.ToList()
                };

                return new WebServiceResult
                {
                    Code = WebServiceCode.Success,
                    DescriptionCode = WebServiceDescriptionCode.None,
                    Data = response
                };
            }

            return new WebServiceResult
            {
                Code = WebServiceCode.Fail,
                DescriptionCode = WebServiceDescriptionCode.None,
                Description = "Can not found patient."
            };
        }

        [Route("api/discharge_instruction")]
        [HttpGet]
        public WebServiceResult GetDischargeInstruction(Guid visitId)
        {
            var html = GetDischargeInstructionHtml(visitId);
            return new WebServiceResult
            {
                Code = WebServiceCode.Success,
                DescriptionCode = WebServiceDescriptionCode.None,
                Description = string.IsNullOrEmpty(html) ? "No instructions are available for this visit." : string.Empty,
                Data = html
            };
        }

        private string GetDischargeInstructionHtml(Guid visitId)
        {
            var dischargeInstruction = _callTransactionService.GetDischargeInstruction(visitId);

            if (!string.IsNullOrEmpty(dischargeInstruction))
            {
                var dischargeModel = JsonConvert.DeserializeObject<DischargeInstructionDataModel>(dischargeInstruction);
                var dischargeHtml = _callTransactionService.FormatDischargeInstructionHtml(_hostingEnvironment.ContentRootPath,
                dischargeModel);
                return dischargeHtml;
            }

            return null;
        }

        [Route("api/last_medication_history")]
        [HttpGet]
        public async Task<WebServiceResult> GetLastPatientMedicationHistory()
        {
            var patientId = await GetCurrentPatientId();

            if (patientId > 0)
            {
                var medicationHistory = await _medicationHistoryService.GetLastPatientMedicationHistory(patientId);

                return new WebServiceResult
                {
                    Code = WebServiceCode.Success,
                    DescriptionCode = WebServiceDescriptionCode.None,
                    Data = medicationHistory ?? EmptyMedicationHistory(patientId)
                };
            }

            return new WebServiceResult
            {
                Code = WebServiceCode.Fail,
                DescriptionCode = WebServiceDescriptionCode.None,
                Description = "Can not found patient."
            };
        }

        private PatientMedicationHistoryModel EmptyMedicationHistory(int patientId)
        {
            return new PatientMedicationHistoryModel
            {
                PatientId = patientId,
                Symptoms = new List<SymptomModel>(),
                Allergies = new List<AllergyModel>(),
                MedicalConditions = new List<MedicalConditionModel>(),
                Surgeries = new List<SurgeryModel>(),
                MedicationHistory = new List<MedicationHistoryModel>(),
                SmokingHistory = new List<SmokingHistoryModel>(),
                AlcoholHistory = new List<AlcoholHistoryModel>(),
                SubstanceAbuseHistory = new List<SubstanceAbuseHistoryModel>(),
            };
        }

        private async Task<int> GetCurrentPatientId()
        {
            var username = User.Identity.Name;
            return await _patientService.GetPatientId(username);
        }
    }
}
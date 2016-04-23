using KAssets.Areas.HelpModule.Models;
using KAssets.Areas.Items.Models;
using KAssets.Models;
using KAssets.Resources.Translation;
using KAssets.Resources.Translation.AssetsTr;
using KAssets.Resources.Translation.InvoiceTr;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Invoices.Models
{
    public class AddInvoiceViewModel
    {
        public int? PoId { get; set; }

        public List<string> ItemIds { get; set; }

        public List<int> Items { get; set; }


        [Required(ErrorMessageResourceName = "InvoiceNumberIsrequired",
            ErrorMessageResourceType = typeof(InvoiceTr))]
        [DenyHtml]
        public string InvoiceNumber { get; set; }

        [Required(ErrorMessageResourceName="RecipientMOLIsRequired",
            ErrorMessageResourceType=typeof(InvoiceTr))]
        [DenyHtml]
        public string RecipientMOL { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        [DenyHtml]
        public string ProviderName { get; set; }

        [Required]
        [DenyHtml]
        public string ProviderEmail { get; set; }

        [Required]
        [DenyHtml]
        public string ProviderAddress { get; set; }

        [Required]
        [DenyHtml]
        public string ProviderBulstat { get; set; }

        [Required]
        [DenyHtml]
        public string BillToSite { get; set; }

        [Required]
        [DenyHtml]
        public string BillToOrganisation{ get; set; }

        [Required]
        [DenyHtml]
        public string BillToAddress { get; set; }

        public List<CurrencyViewModel> Currencies { get; set; }

        public int SelectedCurrency { get; set; }

        [Display(Name="PaymentPeriod",
            ResourceType=typeof(InvoiceTr))]
        [Required(ErrorMessageResourceName = "PaymentPeriodIsRequired",
            ErrorMessageResourceType = typeof(InvoiceTr))]
        public DateTime DateOfPayment { get; set; }
    }

    public class InvoiceListViewModel
    {
        public int Id { get; set; }

        [Display(
        Name = "InvoiceNumber",
        ResourceType = typeof(InvoiceTr))]
        public string InvoiceNumber { get; set; }


        [Display(
        Name = "RemitTo",
        ResourceType = typeof(InvoiceTr))]
        public string RecipientMOL { get; set; }

    }

    public class InvoiceFullViewModel
    {
        public int Id { get; set; }

        [Display(
        Name = "InvoiceNumber",
        ResourceType = typeof(InvoiceTr))]
        public string Number { get; set; }

       [Display(
        Name = "From",
        ResourceType = typeof(Common))]
        public string POFrom { get; set; }

          [Display(
        Name = "Provider",
        ResourceType = typeof(Common))]
        public string POProvider { get; set; }

          [Display(
        Name = "DateOfSend",
        ResourceType = typeof(KAssets.Resources.Translation.ReportsArea.Reports))]
        public string PODateOfSend { get; set; }


        public ICollection<ItemViewModel> Items { get; set; }


        [Display(
        Name = "Price",
        ResourceType = typeof(AssetTr))]
        public string Price { get; set; }
    

        [Display(
        Name = "CompiledUser",
        ResourceType = typeof(InvoiceTr))]
        public string CompiledUser { get; set; }


        [Display(
        Name = "DateOfApproving",
        ResourceType = typeof(InvoiceTr))]
        public string DateOfApproving { get; set; }

        [Display(
        Name = "DateOfPayment",
        ResourceType = typeof(InvoiceTr))]
        public string DayOfPayment { get; set; }

        [Display(
        Name = "DateOfCreation",
        ResourceType = typeof(InvoiceTr))]
        public string DateOfIssue { get; set; }


        [Display(
        Name = "PaymentPeriod", 
        ResourceType = typeof(InvoiceTr))]
        public string PaymentPeriod { get; set; }

       
        public string ProviderName { get; set; }

        public string ProviderEmail { get; set; }

        public string ProviderAddress { get; set; }

        public string ProviderBulstat { get; set; }


        [Display(
        Name = "Site",
        ResourceType = typeof(Common))]
        public string BillToSite { get; set; }

        [Display(
        Name = "Organisation",
        ResourceType = typeof(Common))]
        public string BillToOrganisation { get; set; }

        [Display(
        Name = "Address",
        ResourceType = typeof(Common))]
        public string BillToAddress { get; set; }

        [Display(
        Name = "AcountablePerson",
        ResourceType = typeof(InvoiceTr))]
        public string RecipientMOL { get; set; }

        [Display(
        Name = "IsPaid",
        ResourceType = typeof(InvoiceTr))]
        public bool IsPaid { get; set; }

        [Display(
        Name = "Approved",
        ResourceType = typeof(InvoiceTr))]
        public bool IsApproved { get; set; }
    }
}
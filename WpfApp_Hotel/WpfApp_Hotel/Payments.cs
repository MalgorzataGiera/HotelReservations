//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WpfApp_Hotel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Payments
    {
        public int PaymentID { get; set; }
        public int ReservationID { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime PaymentDate { get; set; }
    
        public virtual Reservations Reservations { get; set; }
    }
}

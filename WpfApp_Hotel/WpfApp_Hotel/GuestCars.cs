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
    
    public partial class GuestCars
    {
        public string LicensePlate { get; set; }
        public int GuestID { get; set; }
    
        public virtual Guests Guests { get; set; }
    }
}
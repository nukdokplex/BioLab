//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BioLab.Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class order_services
    {
        public long id { get; set; }
        public long order { get; set; }
        public long service { get; set; }
    
        public virtual order order1 { get; set; }
        public virtual service service1 { get; set; }
    }
}

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
    
    public partial class order_research_objects
    {
        public long id { get; set; }
        public long order { get; set; }
        public long research_object { get; set; }
    
        public virtual order order1 { get; set; }
        public virtual research_objects research_objects { get; set; }
    }
}
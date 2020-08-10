using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace FYP.Models
{
    public class Stocktaking
    {
        public int Stocktaking_id { get; set; }

        public int User_id { get; set; }

        public string full_name { get; set; }

        public int total_equipment_quantity { get; set; }

        public int total_accessories_quantity { get; set; }

        public DateTime date_created { get; set; }

        public bool archive { get; set; }

        public int diff_equip { get; set; }

        public int diff_accessory { get; set; }

        [Required(ErrorMessage = "Please select a store to stocktake from")]
        public string storage_location { get; set; }
    }
}

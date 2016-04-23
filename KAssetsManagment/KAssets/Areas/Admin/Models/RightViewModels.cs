using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace KAssets.Areas.Admin.Models
{
    public class RightViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Desciption { get; set; }
    }

    public class AddRightViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class EditRightViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }

    public class SelectedRightViewModel
    {
        public RightViewModel Right { get; set; }

        public bool IsSelected { get; set; }
    }

}
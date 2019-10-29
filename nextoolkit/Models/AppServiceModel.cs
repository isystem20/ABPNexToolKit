using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nextoolkit.Models
{
    public class AppServiceModel
    {
        public string appPath { get; set; }
        public string newEntity { get; set; }
        public string referencedEntityNameSpace { get; set; }
        public string project { get; set; }
        public string prefixPermission { get; set; }





        public string appPathDto { get; set; }
        public List<string> optionalNameSpaces { get; set; }
        public string[] importedAttributes { get; set; }

        public string[] appServiceNameSpaces { get; set; }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HIST_AOI.Class
{
    class CModelInfo
    {
        String m_BatchId;
        String m_DjNumber;
        String m_Route;
        String m_Name;
        String m_revision;
        String m_status;
        String m_operation;
        String m_defDisposition;
        String m_part_number;
        String m_item_type;

        public string BatchId { get => m_BatchId; set => m_BatchId = value; }
        public string DjNumber { get => m_DjNumber; set => m_DjNumber = value; }
        public string Route { get => m_Route; set => m_Route = value; }
        public string Name { get => m_Name; set => m_Name = value; }
        public string Revision { get => m_revision; set => m_revision = value; }
        public string Status { get => m_status; set => m_status = value; }
        public string Operation { get => m_operation; set => m_operation = value; }
        public string DefDisposition { get => m_defDisposition; set => m_defDisposition = value; }
        public string Part_number { get => m_part_number; set => m_part_number = value; }
        public string Item_type { get => m_item_type; set => m_item_type = value; }
    }
}

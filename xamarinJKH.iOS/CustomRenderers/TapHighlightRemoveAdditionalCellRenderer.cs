using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using xamarinJKH.Additional;
using xamarinJKH.iOS.CustomRenderers;

[assembly: ExportRenderer(typeof(AdditionalCell), typeof(TapHighlightRemoveAdditionalCellRenderer))]

namespace xamarinJKH.iOS.CustomRenderers
{
   public  class TapHighlightRemoveAdditionalCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            // убираем стиль по-умлочанию - серый цвет вокруг поля ячейки при нажатии
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;
            return cell;
        }
    }
}
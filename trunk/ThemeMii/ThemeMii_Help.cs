/* This file is part of ThemeMii
 * Copyright (C) 2009 Leathl
 * 
 * ThemeMii is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ThemeMii is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System.Windows.Forms;

namespace ThemeMii
{
    public partial class ThemeMii_Help : Form
    {
        public ThemeMii_Help()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.ThemeMii_Icon;
        }

        private void ThemeMii_Help_Load(object sender, System.EventArgs e)
        {
            rtbBasicInstructions.Rtf = Properties.Resources.ThemeMiiBasics;
            CenterToParent();
        }
    }
}

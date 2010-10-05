using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Medo.Windows.Forms {

	public class QButton : System.Windows.Forms.Button {

		private Color _focusedBackColor = SystemColors.Info;
		private Color _focusedForeColor = SystemColors.InfoText;


		private Color _lastBackColor;
		private Color _lastForeColor;


		protected override void OnEnter(EventArgs e) {
			this._lastBackColor = this.BackColor;
			this._lastForeColor = this.ForeColor;
			this.BackColor = this._focusedBackColor;
			this.ForeColor = this._focusedForeColor;
			base.OnEnter(e);
		}

		protected override void OnLeave(EventArgs e) {
			this.BackColor = this._lastBackColor;
			this.ForeColor = this._lastForeColor;
			base.OnLeave(e);
		}

	}
}

//Josip Medved <jmedved@jmedved.com>   www.medo64.com

//2008-12-22: Fixed high DPI mode.
//2008-12-10: Modified to work at higher DPI.
//            Obsoleted FillUsers method use SetUsers instead.
//2008-10-15: First version.


using System;
using System.Collections;
using System.Drawing;
using System.Security;
using System.Windows.Forms;
using System.Security.Principal;

namespace Medo.Windows.Forms {

    /// <summary>
    /// Login dialog.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Login", Justification = "It seems that only Microsoft preferes LogOn.")]
    public class LoginDialog : IDisposable {

        private Form _form;
        private ComboBox cmbUserName;
        private TextBox txtPassword;
        private Button btnOK;

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public LoginDialog() {
        }

        void cmbUserName_Leave(object sender, EventArgs e) {
            if ((cmbUserName.SelectedItem == null) && (!string.IsNullOrEmpty(cmbUserName.Text))) {
                for (int i = 0; i < cmbUserName.Items.Count; ++i) {
                    object iItem = cmbUserName.Items[i];
                    if (iItem.Equals(cmbUserName.Text)) {
                        cmbUserName.SelectedItem = iItem;
                    }
                }
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                if (this.AcceptOnReturn) {
                    if (btnOK.Enabled) {
                        this._form.DialogResult = DialogResult.OK;
                    }
                } else {
                    btnOK.Select();
                }
                e.Handled = true;
            }
        }

        private void cmbUserName_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyData == Keys.Enter) {
                txtPassword.Select();
                e.Handled = true;
            }
        }

        private void cmbUserName_TextChanged(object sender, EventArgs e) {
            btnOK.Enabled = cmbUserName.Text.Length > 0;
        }

        private void frm_Shown(object sender, EventArgs e) {
            if (cmbUserName.Text.Length > 0) { txtPassword.Select(); }
        }

        /// <summary>
        /// Gets/sets whether dialog is closed as soon as return is pressed on password field.
        /// </summary>
        public bool AcceptOnReturn { get; set; }

        private string _defaultUserName = null;

        /// <summary>
        /// Gets/sets name of selected user.
        /// </summary>
        public string UserName {
            get {
                if (_form != null) {
                    return cmbUserName.Text;
                } else {
                    return this._defaultUserName;
                }
            }
            set {
                if (_form != null) {
                    cmbUserName.Text = value;
                } else {
                    this._defaultUserName = value;
                }
            }
        }

        /// <summary>
        /// Gets selected user if such exists. If no user is selected, all items are enumerated and equalitry check is made. If still there is no match, null is returned.
        /// </summary>
        public object SelectedUser {
            get {
                if (_form != null) {
                    if (cmbUserName.SelectedItem != null) {
                        return cmbUserName.SelectedItem;
                    } else {
                        if (!string.IsNullOrEmpty(cmbUserName.Text)) {
                            for (int i = 0; i < cmbUserName.Items.Count; ++i) {
                                object iItem = cmbUserName.Items[i];
                                if (iItem.Equals(cmbUserName.Text)) {
                                    return iItem;
                                }
                            }
                        }
                        return null;
                    }
                } else {
                    return null;
                }
            }
        }

        /// <summary>
        /// Fills list of users.
        /// </summary>
        /// <param name="users">List to fill</param>
        [Obsolete("Use SetUsers instead.", false)]
        public void FillUsers(IEnumerable users) {
            SetUsers(users);
        }

        private IEnumerable _users = null;

        /// <summary>
        /// Fills list of users.
        /// </summary>
        /// <param name="users">List to fill</param>
        public void SetUsers(IEnumerable users) {
            this._users = users;
        }

        /// <summary>
        /// Gets entered password.
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Shows modal dialog.
        /// </summary>
        public DialogResult ShowDialog() {
            return ShowDialog(null);
        }

        /// <summary>
        /// Shows modal dialog.
        /// </summary>
        /// <param name="owner">Any object that implements System.Windows.Forms.IWin32Window that represents the top-level window that will own the modal dialog box.</param>
        public DialogResult ShowDialog(IWin32Window owner) {
            Font defaultFont;
            Form frmOwner = owner as Form;
            if (frmOwner != null) {
                defaultFont = frmOwner.Font;
            } else {
                defaultFont = SystemFonts.MessageBoxFont;
            }


            cmbUserName = new ComboBox();
            cmbUserName.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            cmbUserName.AutoCompleteMode = AutoCompleteMode.Append;
            cmbUserName.AutoCompleteSource = AutoCompleteSource.CustomSource;
            cmbUserName.DropDownStyle = ComboBoxStyle.DropDown;
            cmbUserName.Font = defaultFont;
            cmbUserName.Top = 7;
            cmbUserName.KeyDown += new KeyEventHandler(cmbUserName_KeyDown);
            cmbUserName.TextChanged += new EventHandler(cmbUserName_TextChanged);
            cmbUserName.Leave += new EventHandler(cmbUserName_Leave);

            Label lblUserName = new Label();
            lblUserName.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            lblUserName.AutoSize = true;
            lblUserName.Font = defaultFont;
            lblUserName.Location = new Point(7, cmbUserName.Top + (cmbUserName.Height - lblUserName.Height) / 2 - 1);
            lblUserName.Text = Resources.LabelUserName;

            txtPassword = new TextBox();
            txtPassword.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            txtPassword.Font = defaultFont;
            txtPassword.PasswordChar = '*';
            txtPassword.Top = cmbUserName.Bottom + 7;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.KeyDown += new KeyEventHandler(txtPassword_KeyDown);

            Label lblPassword = new Label();
            lblPassword.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            lblPassword.AutoSize = true;
            lblPassword.Font = defaultFont;
            lblPassword.Location = new Point(7, txtPassword.Top + (txtPassword.Height - lblPassword.Height) / 2 - 1);
            lblPassword.Text = Resources.LabelPassword;

            cmbUserName.Left = System.Math.Max(lblUserName.Right, lblPassword.Right) + 42;
            cmbUserName.Width = lblPassword.Height * 14;
            txtPassword.Left = System.Math.Max(lblUserName.Right, lblPassword.Right) + 42;
            txtPassword.Width = lblPassword.Height * 14;

            Button btnCancel = new Button();
            btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Font = defaultFont;
            btnCancel.Size = new Size(lblPassword.Height * 5, (int)(lblPassword.Height * 1.5));
            btnCancel.Text = Resources.ButtonCancel;

            btnOK = new Button();
            btnOK.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Enabled = false;
            btnOK.Font = defaultFont;
            btnOK.Size = btnCancel.Size;
            btnOK.Text = Resources.ButtonOK;

            this._form = new Form();
            this._form.AutoScaleMode = AutoScaleMode.None;
            this._form.CancelButton = btnCancel;
            this._form.ClientSize = new Size(txtPassword.Right + 7, cmbUserName.Height + txtPassword.Height + btnCancel.Height + 35);
            this._form.Controls.Add(cmbUserName);
            this._form.Controls.Add(lblUserName);
            this._form.Controls.Add(txtPassword);
            this._form.Controls.Add(lblPassword);
            this._form.Controls.Add(btnOK);
            this._form.Controls.Add(btnCancel);
            this._form.Font = defaultFont;
            this._form.FormBorderStyle = FormBorderStyle.FixedDialog;
            this._form.MaximizeBox = false;
            this._form.MinimizeBox = false;
            this._form.ShowIcon = false;
            this._form.ShowInTaskbar = false;
            if (frmOwner != null) {
                this._form.StartPosition = FormStartPosition.CenterParent;
            } else {
                this._form.StartPosition = FormStartPosition.CenterScreen;
            }
            this._form.Text = Resources.Title;
            this._form.Shown += new EventHandler(frm_Shown);

            btnCancel.Location = new Point(this._form.ClientRectangle.Right - btnCancel.Width - 7, this._form.ClientRectangle.Bottom - btnCancel.Height - 7);
            btnOK.Location = new Point(btnCancel.Left - btnOK.Width - 7, this._form.ClientRectangle.Bottom - btnOK.Height - 7);

            if (this._users != null) {
                IEnumerator eUsers = this._users.GetEnumerator();
                while (eUsers.MoveNext()) {
                    object iUser = eUsers.Current;
                    IIdentity iIdentity = iUser as IIdentity;
                    if (iIdentity != null) {
                        cmbUserName.AutoCompleteCustomSource.Add(iIdentity.Name);
                    }
                    cmbUserName.Items.Add(iUser);
                }
            }
            cmbUserName.Text = this._defaultUserName;


            txtPassword.Text = "";

            if (this._form.ShowDialog(owner) == DialogResult.OK) {
                this.UserName = cmbUserName.Text;
                this.Password = txtPassword.Text;
                return DialogResult.OK;
            } else {
                this.UserName = null;
                this.Password = null;
                return DialogResult.Cancel;
            }
        }


        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this._form.Dispose();
                this.cmbUserName.Dispose();
                this.txtPassword.Dispose();
                this.btnOK.Dispose();
            }
        }

        #endregion


        private static class Resources {

            internal static string Title {
                get { return GetInCurrentLanguage("Login", "Prijava"); }
            }

            internal static string LabelUserName {
                get { return GetInCurrentLanguage("User name:", "Korisničko ime:"); }
            }

            internal static string LabelPassword {
                get { return GetInCurrentLanguage("Password:", "Lozinka:"); }
            }

            internal static string ButtonOK {
                get { return GetInCurrentLanguage("OK", "U redu"); }
            }

            internal static string ButtonCancel {
                get { return GetInCurrentLanguage("Cancel", "Odustani"); }
            }


            private static string GetInCurrentLanguage(string en_US, string hr_HR) {
                switch (System.Threading.Thread.CurrentThread.CurrentUICulture.Name.ToUpperInvariant()) {
                    case "EN":
                    case "EN-US":
                    case "EN-GB":
                        return en_US;

                    case "HR":
                    case "HR-HR":
                    case "HR-BA":
                        return hr_HR;

                    default:
                        return en_US;
                }
            }

        }

    }

}

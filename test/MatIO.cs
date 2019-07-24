using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDAMes.Special
{
    public partial class MatIO : Form
    {
        public MatIO()
        {
            InitializeComponent();

            SetLayout();
            SetEventHandler();

            picQty.Image = imageList.Images[0];
            this.WindowState = FormWindowState.Maximized;

        }
        /// <summary>
        /// 초기화

        /// </summary>
        private void SetLayout()
        {
            ClearForm();
        }

        /// <summary>
        /// 이벤트 정의
        /// </summary>
        private void SetEventHandler()
        {
            userControl_Button.ExecuteClick += new EventHandler(userControl_Button_ExecuteClick);
            userControl_Button.CloseClick += new EventHandler(userControl_Button_CloseClick);
            userControl_Button.ResetClick += new EventHandler(userControl_Button_ResetClick);

            optIn.CheckedChanged += new EventHandler(optIn_CheckedChanged);
            optOut.CheckedChanged += new EventHandler(optOut_CheckedChanged);

            txtBarcode.KeyPress += new KeyPressEventHandler(txtBarcode_KeyPress);

            picQty.MouseDown += new MouseEventHandler(picQty_MouseDown);
            picQty.MouseUp += new MouseEventHandler(picQty_MouseUp);
        }

        void picQty_MouseUp(object sender, MouseEventArgs e)
        {
            picQty.Image = imageList.Images[0];
            if (txtLot.Text != string.Empty)
            {
                Popup.KeyPad keypad = new PDAMes.Popup.KeyPad();
                keypad.ShowDialog();
                txtQty.Text = keypad.QtyData;
            }
        }

        void picQty_MouseDown(object sender, MouseEventArgs e)
        {
            picQty.Image = imageList.Images[1];
        }

        void txtBarcode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txtBarcode.Text = txtBarcode.Text.ToUpper();

                try
                {
                    EqpHelper eh = new EqpHelper();

                    if (txtBarcode.Text.IndexOf("/") == -1)
                        return;

                    string[] barcode = txtBarcode.Text.Split(new char[] { '/' });

                    txtMatKind.Text = eh.GetMstItem(barcode[0]);

                    if (txtMatKind.Text == string.Empty)
                        MessageBox.Show("해당 자재코드는 ERP에 등록된 자재가 아닙니다.", "미등록 자재");
                    else
                    {

                        txtItem.Text = barcode[0];
                        txtLot.Text = barcode[1];
                        txtQty.Text = barcode[2];
                        txtExpire.Text = barcode[3];
                    }
                }
                catch (Exception)
                {
                }

                txtBarcode.Text = string.Empty;
            }

        }

        void optOut_CheckedChanged(object sender, EventArgs e)
        {
            txtBarcode.Focus();
        }

        void optIn_CheckedChanged(object sender, EventArgs e)
        {
            txtBarcode.Focus();
        }

        void userControl_Button_ResetClick(object sender, EventArgs e)
        {
            SetLayout();
        }

        void userControl_Button_CloseClick(object sender, EventArgs e)
        {
            this.Close();
        }

        void userControl_Button_ExecuteClick(object sender, EventArgs e)
        {
            SetExecuteData();
        }

        /// <summary>
        /// Execute Data
        /// </summary>
        void SetExecuteData()
        {
            Cursor.Current = Cursors.WaitCursor;

            if (Validation())
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    string ioflag = string.Empty;

                    if (optIn.Checked)
                        ioflag = "I";
                    else
                        ioflag = "O";

                    sb.Append("insert into erp_mathst \n");
                    sb.Append(" (plant, \n");
                    sb.Append("  erp_lotno, \n");
                    sb.Append("  trans_time, \n");
                    sb.Append("  io_flag, \n");
                    sb.Append("  erp_material_kind, \n");
                    sb.Append("  erp_itemcode, \n");
                    sb.Append("  erp_qty, \n");
                    sb.Append("  erp_expire, \n");
                    sb.Append("  user_id) \n");
                    sb.Append("values \n");
                    sb.Append(" ('" + PDALib.plant + "', \n");
                    sb.Append("  '" + txtLot.Text + "', \n");
                    sb.Append("  to_char(sysdate,'yyyymmddhh24miss'), \n");
                    sb.Append("  '" + ioflag + "', \n");
                    sb.Append("  '" + txtMatKind.Text + "', \n");
                    sb.Append("  '" + txtItem.Text + "', \n");
                    sb.Append("  '" + txtQty.Text + "', \n");
                    sb.Append("  '" + txtExpire.Text + "', \n");
                    sb.Append("  '" + PDALib.user_id + "')");

                    //., 소스제외(사용안하는 소스 ):
                    //.wsQuery.ExecuteQuery(sb.ToString());
                    PDALib.ShowMessage("작업이 정상적으로 처리 되었습니다.", PDALib.TRANSACTION_MESSAGE_TIME);

                    SetLayout();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            Cursor.Current = Cursors.Default;

        }

        /// <summary>
        /// Validation
        /// </summary>
        /// <returns></returns>
        bool Validation()
        {
            if (string.IsNullOrEmpty(txtLot.Text))
            {
                MessageBox.Show("자재 Lot 정보가 누락되었습니다.", "Lot 누락");
                txtLot.Focus();

                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Clear
        /// </summary>
        void ClearForm()
        {
            foreach (Control ctl in Controls)
            {
                if (ctl is TextBox)
                    ctl.Text = string.Empty;
            }
        }
    }
}
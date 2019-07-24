테스트 소스 수정한다
나도 소스 수정했당 git_tset
너도 소스 수정해봐랑
using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDAMes.Popup
{
    public partial class CheckBOM : Form
    {
        string plant = string.Empty;

        LotHelper _cLotHelper = new LotHelper();

        StringBuilder sb = new StringBuilder();
        DataTable dtgridBOM = new DataTable();
        #region  << 디비 컨트롤 >>
        private DBControl _dbControl = new DBControl();
        #endregion


        public CheckBOM()
        {
            InitializeComponent();

            SetLayout();
            SetEventHandler();
        }

        public CheckBOM(string LotNumber, string OperCode, string OperName)
        {
            InitializeComponent();

            SetLayout();
            SetEventHandler();

            txtLot.Text = LotNumber;
            txtOperCode.Text = OperCode;
            txtOperName.Text = OperName;

            SearchBom();
        }


        /// <summary>
        /// 초기화
        /// </summary>
        private void SetLayout()
        {
            plant = PDALib.plant;

            txtLot.Text = "";
            txtOperCode.Text = "";
            txtOperName.Text = "";
            gridBOM.DataSource = null;
            gridBOM.TableStyles[0].GridColumnStyles.Clear();
            gridBOM.Refresh();
        }

        /// <summary>
        /// 이벤트 정의
        /// </summary>
        private void SetEventHandler()
        {
            this.Closing += new CancelEventHandler(CheckBOM_Closing);

            txtLot.KeyPress += new KeyPressEventHandler(txtLot_KeyPress);

            gridBOM.CurrentCellChanged += new EventHandler(gridBOM_CurrentCellChanged);

            userControl_Button.ExecuteClick += new EventHandler(userControl_Button_ExecuteClick);
            userControl_Button.ResetClick += new EventHandler(userControl_Button_ResetClick);
            userControl_Button.CloseClick += new EventHandler(userControl_Button_CloseClick);
        }

        /// <summary>
        /// Form Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void userControl_Button_CloseClick(object sender, EventArgs e)
        {
            //this.Close();
        }

        /// <summary>
        /// Reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void userControl_Button_ResetClick(object sender, EventArgs e)
        {
            //SetLayout();
        }

        void txtLot_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                SearchBom();
            }
        }

        /// <summary>
        /// CurrentCellChanged
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void gridBOM_CurrentCellChanged(object sender, EventArgs e)
        {
            if (gridBOM.CurrentCell.ColumnNumber == 1)
            {
                string tempstr = string.Empty;

                for (int count = 0; count < gridBOM.VisibleRowCount; count++)
                {
                    Popup.InputTextBox txtbox = new PDAMes.Popup.InputTextBox();
                    txtbox.ShowDialog();

                    tempstr = txtbox.txtData;

                    gridBOM[gridBOM.CurrentCell.RowNumber, gridBOM.CurrentCell.ColumnNumber] = tempstr;

                    count++;

                    if(count == gridBOM.VisibleRowCount)
                    {
                        gridBOM.CurrentCell = new DataGridCell(0, 0);
                        userControl_Button_ExecuteClick(null, null);
                        break;
                    }
                    else
                    {
                        gridBOM.CurrentCell = new DataGridCell(0, 0);
                    }
                }
            }
            //userControl_Button_ExecuteClick(sender, null);
        }

        /// <summary>
        /// Form Close
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CheckBOM_Closing(object sender, CancelEventArgs e)
        {

        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void userControl_Button_ExecuteClick(object sender, EventArgs e)
        {
            InterLock interlock = new InterLock();
            interlock.GetInterLock();

            SetExecuteData();
        }


        void SearchBom()
        {
            try
            {
                if (_cLotHelper.GetLotInfo(txtLot.Text, this.Name))
                {

                    sb.Remove(0, sb.Length);

                    sb.Append("SELECT DISTINCT(ERP_MATERIAL_KIND) AS 종류 \n");
                    sb.Append("FROM ERPBOM  (NOLOCK) \n");
                    sb.Append("WHERE PLANT  ='" + PDALib.plant + "' \n");
                    sb.Append("  AND PART_ID IN \n");
                    sb.Append("     (SELECT PART FROM LOTSTS  (NOLOCK) WHERE PLANT='JCPKG1' AND LOT_NUMBER='" + txtLot.Text + "')\n");
                    sb.Append("         AND OPERATION_CODE = '" + LotHelper.oper_code + "' \n");
                    sb.Append("         AND ERP_MATERIAL_KIND IN   \n");
                    sb.Append("                            ( SELECT SYSCODE_DESC AS MATERIAL_KIND  \n");
                    sb.Append("                                FROM SYSCODEDATA    (NOLOCK)        \n");
                    sb.Append("                               WHERE PLANT='JCPKG1'                 \n");
                    sb.Append("                                 AND SYSTABLE_NAME='BOM_OPER_CHK'   \n");
                    sb.Append("                                 AND SYSCODE_SEQ=1                  \n");
                    sb.Append("                                 AND SYSCODE_GROUP='" + LotHelper.oper_code + "')  \n");

                    dtgridBOM = _dbControl.Inquiry(sb.ToString());


                    gridBOM.Refresh();

                    if (dtgridBOM.Rows.Count != 0)
                    {
                        gridBOM.DataSource = null;
                        gridBOM.TableStyles[0].GridColumnStyles.Clear();
                        gridBOM.Refresh();

                        dtgridBOM.Columns.Add(new DataColumn("ITEM_CODE", typeof(string)));

                        gridBOM.DataSource = dtgridBOM;

                        gridBOM.Refresh();

                        for (int i = 0; i < dtgridBOM.Rows.Count; i++)
                        {
                            string tempstr = string.Empty;
                            Popup.InputTextBox txtbox = new PDAMes.Popup.InputTextBox();
                            txtbox.ShowDialog();

                            tempstr = txtbox.txtData;

                            //string[] item_code = gridBOM[count, 1].ToString().Split('/');

                            sb.Remove(0, sb.Length);

                            //sb.Append("SELECT MATERIAL_KIND FROM TPO_MST_ITEM_VIEW WHERE PLANT='SEMITEQ' AND ITEMCODE='" + item_code[0] + "'");

                            
                            gridBOM[i, 1] = tempstr;
                        }

                        userControl_Button_ExecuteClick(null, null);
                    }
                    else
                    {
                        PDALib.InterLockSound();
                        MessageBox.Show(txtLot.Text + "기준정보에 등록된 BOM이 없습니다.", "BOM 정보 없음");
                        this.Close();
                        return;
                    }
                }
            }

            catch (Exception ex)
            {
                PDALib.InterLockSound();
                MessageBox.Show(ex.Message);
            }
        }

        bool Validation()
        {
            bool result = false;
            DataTable dtCheckBOM = new DataTable();
            DataTable dtCheckAging = new DataTable();
            DataTable dt = new DataTable();

            try
            {
                for (int count = 0; count < gridBOM.VisibleRowCount; count++)
                {
                    dtCheckBOM.Clear();
                    string[] item_code = gridBOM[count, 1].ToString().Split('/');

                    sb.Remove(0, sb.Length);

                    sb.Append("SELECT ITEM_CODE \n");
                    sb.Append("FROM ERPBOM  (NOLOCK) \n");
                    sb.Append(string.Format("WHERE PLANT  ='{0}' \n", "JCPKG1"));
                    sb.Append("  AND PART_ID IN \n");
                    sb.Append(string.Format("    (SELECT PART FROM LOTSTS  (NOLOCK) WHERE PLANT='{0}' AND LOT_NUMBER='{1}')", "JCPKG1", txtLot.Text));
                    sb.Append("  AND OPERATION_CODE = '" + LotHelper.oper_code + "' \n");
                    sb.Append("  AND ERP_ITEMCODE = '" + item_code[0] + "' \n");
                    sb.Append("  AND ERP_MATERIAL_KIND = '" + gridBOM[count, 0].ToString() + "' \n");

                    
                    dtCheckBOM = _dbControl.Inquiry(sb.ToString());

                    if (dtCheckBOM.Rows.Count > 0)
                    {
                        result = true;
                    }
                    else
                    {
                        if (item_code[0].Equals(""))
                        {
                            PDALib.InterLockSound();
                            MessageBox.Show("[Validation] " + gridBOM[count, 0].ToString() + " : 입력되지 않았습니다.", "CheckBOM");
                        }
                        else
                        {
                            PDALib.InterLockSound();
                            MessageBox.Show("[Validation] " + gridBOM[count, 0].ToString() + " : " + item_code[0] + " - 부적합자재입니다.", "CheckBOM");
                        }
                        result = false;

                        gridBOM.CurrentCell = new DataGridCell(0, 0);
                        break;
                    }

                    if (!_cLotHelper.CompareDate(item_code[3].ToString()))
                    {
                        PDALib.InterLockSound();
                        MessageBox.Show("[Validation] " + item_code[0].ToString() + " 해당 자재코드는 유효기간이 지났습니다..", "유효일 경과 자재");
                        gridBOM.CurrentCell = new DataGridCell(0, 0);
                        result = false;
                        break;
                    }

                    //20181030 GUN EMC Aging 세부 Interlock 추가
                    if (item_code[0].ToString().Substring(0, 3).Equals("MEM"))
                    {
                        sb.Remove(0, sb.Length);

                        sb.Append("SELECT USE_FLAG, sign(convert(NUMERIC(10), use_time - getdate()))  as use_time_flag \n");
                        sb.Append("FROM SQ_MATLIFE_LOT  (NOLOCK) \n");
                        sb.Append(string.Format("WHERE PLANT  = '{0}' \n", PDALib.plant));
                        sb.Append("  AND MAT_LOTNO = '" + item_code[1] + "' \n");
                        sb.Append("  AND OPERATION = '4000' \n");
                        sb.Append("  AND MAT_ITEMCODE = '" + item_code[0] + "' \n");
                        sb.Append("  AND MAT_KIND = 'EMC' \n");


                        dtCheckAging = _dbControl.Inquiry(sb.ToString());

                        if (dtCheckAging.Rows.Count == 0)
                        {
                            PDALib.InterLockSound();
                            MessageBox.Show("[Validation] 해당 자재는 AGING 미시작 자재입니다..", "AGING 미시작 자재");

                            gridBOM.CurrentCell = new DataGridCell(0, 0);
                            result = false;
                            break;
                        }
                        else
                        {
                            if (dtCheckAging.Rows[0][0].ToString() == "N")
                            {
                                PDALib.InterLockSound();
                                MessageBox.Show("[Validation] 해당 자재는 AGING 미완료 자재입니다..", "AGING 미완료 자재");

                                gridBOM.CurrentCell = new DataGridCell(0, 0);
                                result = false;
                                break;
                            }
                            else if (Convert.ToInt32(dtCheckAging.Rows[0][1].ToString()) == -1)
                            {
                                PDALib.InterLockSound();
                                MessageBox.Show("[Validation] 자재의 사용기간이 지났습니다..", "EMC 자재 확인");
                                
                                gridBOM.CurrentCell = new DataGridCell(0, 0);
                                result = false;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                PDALib.InterLockSound();
                MessageBox.Show("[Validation] " + ex.Message);
            }
            return result;
        }

        void SetExecuteData()
        {
            if (Validation())
            {
                try
                {
                    //MessageBox.Show("[SetExecuteData] BOMCHECK가 정상 진행되었습니다.");
                    LotHelper.CheckBomResult = true;
                    this.Close();
                }
                catch (Exception ex)
                {
                    PDALib.InterLockSound();
                    MessageBox.Show("[SetExecuteData] " + ex.Message);
                }
            }
        }
    }
}
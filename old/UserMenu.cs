old도 수정한당 
트런크는 어떻게 테스트하지
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PDAMes.Special
{
    public partial class UserMenu : Form
    {
        private UserMenuHelper menuHelper = new UserMenuHelper();

        public UserMenu()
        {
            InitializeComponent();

            SetLayout();
            SetEventHandler();

            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// 초기 화면
        /// </summary>
        private void SetLayout()
        {
            picSet.Image = imageListExe.Images[0];
            picClose.Image = imageListExe.Images[2];

            GetAllMenu();
//            GetUserMenu();
        }

        /// <summary>
        /// 이벤트 정의
        /// </summary>
        private void SetEventHandler()
        {
            lstAllMenu.KeyPress += new KeyPressEventHandler(lstAllMenu_KeyPress);
            lstUserMenu.KeyPress += new KeyPressEventHandler(lstUserMenu_KeyPress);

            picSet.MouseUp += new MouseEventHandler(picSet_MouseUp);
            picSet.MouseDown += new MouseEventHandler(picSet_MouseDown);
            picClose.MouseUp += new MouseEventHandler(picClose_MouseUp);
            picClose.MouseDown += new MouseEventHandler(picClose_MouseDown);
            picLeft.Click += new EventHandler(picLeft_Click);
            picRight.Click += new EventHandler(picRight_Click);
            picLeftAll.Click += new EventHandler(picLeftAll_Click);
            picRightAll.Click += new EventHandler(picRightAll_Click);

        }

        /// <summary>
        /// 엔터키로 아이템 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lstUserMenu_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                MoveList(lstUserMenu, lstAllMenu);
        }

        /// <summary>
        /// 엔터키로 아이템 이동
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void lstAllMenu_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
                MoveList(lstAllMenu, lstUserMenu);

        }

        /// <summary>
        /// 사용자 메뉴 전체 등록
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void picRightAll_Click(object sender, EventArgs e)
        {
            int itemcount = lstAllMenu.Items.Count;

            for (int i = 0; i < itemcount; i++)
            {
                lstAllMenu.SelectedIndex = 0;
                MoveList(lstAllMenu, lstUserMenu);
            }
        }

        /// <summary>
        /// 사용자 메뉴 전체 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void picLeftAll_Click(object sender, EventArgs e)
        {
            int itemcount = lstUserMenu.Items.Count;

            for (int i = 0; i < itemcount; i++)
            {
                lstUserMenu.SelectedIndex = 0;
                MoveList(lstUserMenu, lstAllMenu);
            }
        }

        /// <summary>
        /// 사용자 메뉴 등록
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void picRight_Click(object sender, EventArgs e)
        {
            MoveList(lstAllMenu, lstUserMenu);
        }

        /// <summary>
        /// 사용자 메뉴 삭제
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void picLeft_Click(object sender, EventArgs e)
        {
            MoveList(lstUserMenu, lstAllMenu);
        }

        /// <summary>
        /// 폼 종료
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void picClose_MouseDown(object sender, MouseEventArgs e)
        {
            picClose.Image = imageListExe.Images[3];
        }

        void picClose_MouseUp(object sender, MouseEventArgs e)
        {
            picClose.Image = imageListExe.Images[2];
            this.Close();
        }

        /// <summary>
        /// 저장
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void picSet_MouseDown(object sender, MouseEventArgs e)
        {
            picSet.Image = imageListExe.Images[1];
        }

        void picSet_MouseUp(object sender, MouseEventArgs e)
        {
            PDALib pdalib = new PDALib();

            picSet.Image = imageListExe.Images[0];
            List<FavorityObject> favorityList = new List<FavorityObject>();

            foreach (FavorityObject fo in lstAllMenu.Items)
            {
                fo.USE_FLAG = "N";
                favorityList.Add(fo);
            }
            foreach (FavorityObject fo in lstUserMenu.Items)
            {
                fo.USE_FLAG = "Y";
                favorityList.Add(fo);
            }

             
            if (menuHelper.UpdateFavorites(favorityList) == 1)
                PDALib.ShowMessage("저장되었습니다", 1000);
            else
                PDALib.ShowMessage("저장이 실패 하였습니다.", 1000);

            this.Close();
        }

        /// <summary>
        /// 리스트박스 아이템 이동
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Target"></param>
        private void MoveList(ListBox Source, ListBox Target)
        {
            if (Source.SelectedIndex < 0)
                return;

            Target.Items.Add(Source.Items[Source.SelectedIndex]);
            Source.Items.RemoveAt(Source.SelectedIndex);
        }

        /// <summary>
        /// 사용자 메뉴 문자열
        /// </summary>
        /// <returns></returns>
        private string GetList()
        {
            string rtn = string.Empty;

            if (lstUserMenu.Items.Count > 0)
            {
                
                for (int i = 0; i < lstUserMenu.Items.Count; i++)
                {
                    rtn += lstUserMenu.Items[i].ToString() + "|";
                }

                rtn = rtn.Substring(0, rtn.Length - 1);
            }

            return rtn;
        }

        ///// <summary>
        ///// 등록된 사용자 메뉴
        ///// </summary>
        //private void GetUserMenu()
        //{
        //    PDALib pdalib = new PDALib();

        //    string temp = pdalib.GetConfigXml("config", "user", "eqp");

        //    lstUserMenu.Items.Clear();

        //    if (temp != string.Empty)
        //    {
        //        string[] menulist = temp.Split(new char[] { '|' });

        //        foreach (string menu in menulist)
        //        {
        //            for (int i = 0; i < lstAllMenu.Items.Count; i++)
        //            {
        //                if (menu == lstAllMenu.Items[i].ToString())
        //                {
        //                    lstAllMenu.SelectedIndex = i;
        //                    MoveList(lstAllMenu, lstUserMenu);
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// 전체 메뉴
        /// </summary>
        private void GetAllMenu()
        {

            menuHelper.UpdateFavoritesToCurretnVersion();

            DataTable dtFavorites = menuHelper.GetFavorites();
            if (dtFavorites.Rows.Count < 1)
            {
                menuHelper.AddAllFavorites();
                dtFavorites = menuHelper.GetFavorites();
            }


            lstAllMenu.Items.Clear();
            lstUserMenu.Items.Clear();

            
            // 데이터 입력 
            FavorityObject fo;
            foreach (DataRow dr in dtFavorites.Rows)
            {
                fo = new FavorityObject(dr[2].ToString(), dr[1].ToString(), int.Parse(dr[0].ToString()));
                if (fo.USE_FLAG == "Y")
                    lstUserMenu.Items.Add(fo);
                else
                    lstAllMenu.Items.Add(fo);                        
            }
            
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace coinoneAlarm
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        Sources.Common.EncryptData userInfo = new Sources.Common.EncryptData();

        private System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        private System.Windows.Forms.ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
        private System.Windows.Forms.MenuItem menuItemOpen = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.MenuItem menuItemSettings = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.MenuItem menuItemClose = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.MenuItem menuItemExit = new System.Windows.Forms.MenuItem();
        private System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        private bool _isEnableNoticeBtc;
        private bool _isEnableNoticeBch;
        private bool _isEnableNoticeEth;
        private bool _isEnableNoticeEtc;
        private bool _isEnableNoticeXrp;
        private bool _isEnableNoticeQtum;
        private bool _isEnableNoticeLtc;
        private bool _isEnableNoticeIota;
        private bool _isEnableNoticeBtg;

        private bool _isAuth;

        private int _tickCount;
        
        private enum CoinList
        {
            BTC,
            BCH,
            ETH,
            ETC,
            XRP,
            QTUM,
            LTC,
            IOTA,
            BTG
        }

        #region Init & Set Default State

        public MainWindow()
        {
            InitializeComponent();

            SetContextMenu();
            SetNotifyIcon();
            SetInitValues();
            SetAccessControls();
            SetTopControls(false);
            SetTimer();

            userInfo.GenerateRandomString();
        }

        private void SetTimer()
        {
            timer.Interval = 10001;
            timer.Tick += new EventHandler(TimerTick);
            timer.Start();

            TimerTick(null, null);
        }

        private void SetNotifyIcon()
        {
            notifyIcon.Icon = Properties.Resources.coinone_icon;
            notifyIcon.ContextMenu = contextMenu;
            notifyIcon.Visible = true;

            notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(NotifyIconMouseDoubleClick);
        }

        private void SetContextMenu()
        {
            menuItemOpen.Text = Properties.Resources.MENU_OPEN;
            menuItemClose.Text = Properties.Resources.MENU_CLOSE;
            menuItemExit.Text = Properties.Resources.MENU_EXIT;

            contextMenu.MenuItems.Add(menuItemOpen);
            contextMenu.MenuItems.Add(menuItemClose);
            contextMenu.MenuItems.Add(menuItemExit);

            menuItemOpen.Click += new EventHandler(MenuItemMouseClick);
            menuItemClose.Click += new EventHandler(MenuItemMouseClick);
            menuItemExit.Click += new EventHandler(MenuItemMouseClick);
        }

        private void SetInitValues()
        {
            _tickCount = 0;

            _isAuth = false;

            _isEnableNoticeBtc = false;
            _isEnableNoticeBch = false;
            _isEnableNoticeEth = false;
            _isEnableNoticeEtc = false;
            _isEnableNoticeXrp = false;
            _isEnableNoticeQtum = false;
            _isEnableNoticeLtc = false;
            _isEnableNoticeIota = false;
            _isEnableNoticeBtg = false;
        }

        private void SetAccessControls()
        {
            tbAccessKeyInput.Password = "fa3b24ac-7918-4f26-9a2c-80a9f23dd197";
            tbAccessKeyInput.IsEnabled = false;
        }

        private void SetTopControls(bool isAuthorized)
        {
            if (isAuthorized == true)
            {
                tbAccessKey.Visibility = Visibility.Hidden;
                tbAccessKeyInput.Visibility = Visibility.Hidden;
                tbAccessToken.Visibility = Visibility.Hidden;
                tbAccessTokenInput.Visibility = Visibility.Hidden;
                btAuth.Visibility = Visibility.Hidden;

                tbAccessed.Visibility = Visibility.Visible;

                borderTop.Background = Brushes.ForestGreen;
            }
            else
            {
                tbAccessKey.Visibility = Visibility.Visible;
                tbAccessKeyInput.Visibility = Visibility.Visible;
                tbAccessToken.Visibility = Visibility.Visible;
                tbAccessTokenInput.Visibility = Visibility.Visible;
                btAuth.Visibility = Visibility.Visible;

                tbAccessed.Visibility = Visibility.Hidden;

                borderTop.Background = Brushes.DarkGray;
            }
        }

        #endregion

        #region Windows User Event

        private void NotifyIconMouseDoubleClick(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }

        private void MenuItemMouseClick(object sender, EventArgs e)
        {
            System.Windows.Forms.MenuItem selectedItem = (System.Windows.Forms.MenuItem)sender;

            if (selectedItem == menuItemOpen)
                this.Visibility = Visibility.Visible;
            else if (selectedItem == menuItemClose)
                this.Visibility = Visibility.Hidden;
            else if (selectedItem == menuItemExit)
            {
                notifyIcon.Visible = false;
                Application.Current.Shutdown();
            }
            else
                return;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _tickCount++;

            Sources.CoinoneApi coinoneApi = new Sources.CoinoneApi();

            coinoneApi.GetCoinPrice(tbBtcPrice, tbBtcPriceMax, tbBtcPriceMin, "btc");
            coinoneApi.GetCoinPrice(tbBchPrice, tbBchPriceMax, tbBchPriceMin, "bch");
            coinoneApi.GetCoinPrice(tbEthPrice, tbEthPriceMax, tbEthPriceMin, "eth");
            coinoneApi.GetCoinPrice(tbEtcPrice, tbEtcPriceMax, tbEtcPriceMin, "etc");
            coinoneApi.GetCoinPrice(tbXrpPrice, tbXrpPriceMax, tbXrpPriceMin, "xrp");
            coinoneApi.GetCoinPrice(tbQtumPrice, tbQtumPriceMax, tbQtumPriceMin, "qtum");
            coinoneApi.GetCoinPrice(tbLtcPrice, tbLtcPriceMax, tbLtcPriceMin, "ltc");
            coinoneApi.GetCoinPrice(tbIotaPrice, tbIotaPriceMax, tbIotaPriceMin, "iota");
            coinoneApi.GetCoinPrice(tbBtgPrice, tbBtgPriceMax, tbBtgPriceMin, "btg");

            if (_isAuth == true)
            {
                coinoneApi.GetCoinBalance(userInfo.DecryptString(true),
                                          userInfo.DecryptString(false),
                                          tbBtcBalance, tbBchBalance, tbEthBalance, tbEtcBalance, tbXrpBalance,
                                          tbQtumBalance, tbLtcBalance, tbIotaBalance, tbBtgBalance, tbAccessed);
            }

            //alarm per 5 minutes (10sec * 30)
            if (_tickCount >= 30)
            {
                SetNoticeMessage(tbBtcPrice, tbBtcNoticePrice, _isEnableNoticeBtc, CoinList.BTC);
                SetNoticeMessage(tbBchPrice, tbBchNoticePrice, _isEnableNoticeBch, CoinList.BCH);
                SetNoticeMessage(tbEthPrice, tbEthNoticePrice, _isEnableNoticeEth, CoinList.ETH);
                SetNoticeMessage(tbEtcPrice, tbEtcNoticePrice, _isEnableNoticeEtc, CoinList.ETC);
                SetNoticeMessage(tbXrpPrice, tbXrpNoticePrice, _isEnableNoticeXrp, CoinList.XRP);
                SetNoticeMessage(tbQtumPrice, tbQtumNoticePrice, _isEnableNoticeQtum, CoinList.QTUM);
                SetNoticeMessage(tbLtcPrice, tbLtcNoticePrice, _isEnableNoticeLtc, CoinList.LTC);
                SetNoticeMessage(tbIotaPrice, tbIotaNoticePrice, _isEnableNoticeIota, CoinList.IOTA);
                SetNoticeMessage(tbBtgPrice, tbBtgNoticePrice, _isEnableNoticeBtg, CoinList.BTG);

                _tickCount = 0;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Visibility = Visibility.Hidden;
        }

        private void ButtonNoticeClick(object sender, RoutedEventArgs e)
        {
            // button content - image
            object selectedContent = ((Button)sender).Content;
            
            // resource flag - set enable notice & set new image
            //btc
            if (selectedContent == FindResource("noticeOnBtc"))
            {
                _isEnableNoticeBtc = false;
                ((Button)sender).Content = FindResource("noticeOffBtc");
            }
            else if (selectedContent == FindResource("noticeOffBtc"))
            {
                _isEnableNoticeBtc = true;
                ((Button)sender).Content = FindResource("noticeOnBtc");
            }
            //bch
            else if (selectedContent == FindResource("noticeOnBch"))
            {
                _isEnableNoticeBch = false;
                ((Button)sender).Content = FindResource("noticeOffBch");
            }
            else if (selectedContent == FindResource("noticeOffBch"))
            {
                _isEnableNoticeBch = true;
                ((Button)sender).Content = FindResource("noticeOnBch");
            }
            //eth
            else if (selectedContent == FindResource("noticeOnEth"))
            {
                _isEnableNoticeEth = false;
                ((Button)sender).Content = FindResource("noticeOffEth");
            }
            else if (selectedContent == FindResource("noticeOffEth"))
            {
                _isEnableNoticeEth = true;
                ((Button)sender).Content = FindResource("noticeOnEth");
            }
            //etc
            else if (selectedContent == FindResource("noticeOnEtc"))
            {
                _isEnableNoticeEtc = false;
                ((Button)sender).Content = FindResource("noticeOffEtc");
            }
            else if (selectedContent == FindResource("noticeOffEtc"))
            {
                _isEnableNoticeEtc = true;
                ((Button)sender).Content = FindResource("noticeOnEtc");
            }
            //xrp
            else if (selectedContent == FindResource("noticeOnXrp"))
            {
                _isEnableNoticeXrp = false;
                ((Button)sender).Content = FindResource("noticeOffXrp");
            }
            else if (selectedContent == FindResource("noticeOffXrp"))
            {
                _isEnableNoticeXrp = true;
                ((Button)sender).Content = FindResource("noticeOnXrp");
            }
            //qtum
            else if (selectedContent == FindResource("noticeOnQtum"))
            {
                _isEnableNoticeQtum = false;
                ((Button)sender).Content = FindResource("noticeOffQtum");
            }
            else if (selectedContent == FindResource("noticeOffQtum"))
            {
                _isEnableNoticeQtum = true;
                ((Button)sender).Content = FindResource("noticeOnQtum");
            }
            //ltc
            else if (selectedContent == FindResource("noticeOnLtc"))
            {
                _isEnableNoticeLtc = false;
                ((Button)sender).Content = FindResource("noticeOffLtc");
            }
            else if (selectedContent == FindResource("noticeOffLtc"))
            {
                _isEnableNoticeLtc = true;
                ((Button)sender).Content = FindResource("noticeOnLtc");
            }
            //iota
            else if (selectedContent == FindResource("noticeOnIota"))
            {
                _isEnableNoticeIota = false;
                ((Button)sender).Content = FindResource("noticeOffIota");
            }
            else if (selectedContent == FindResource("noticeOffIota"))
            {
                _isEnableNoticeIota = true;
                ((Button)sender).Content = FindResource("noticeOnIota");
            }
            //btg
            else if (selectedContent == FindResource("noticeOnBtg"))
            {
                _isEnableNoticeBtg = false;
                ((Button)sender).Content = FindResource("noticeOffBtg");
            }
            else if (selectedContent == FindResource("noticeOffBtg"))
            {
                _isEnableNoticeBtg = true;
                ((Button)sender).Content = FindResource("noticeOnBtg");
            }
            else return;
        }

        private void ButtonAuthClick(object sender, RoutedEventArgs e)
        {
            if (tbAccessKeyInput.Password.Length < 1 || tbAccessTokenInput.Password.Length < 1) return;
            
            userInfo.EncryptString(tbAccessKeyInput.Password, true);
            userInfo.EncryptString(tbAccessTokenInput.Password, false);

            SetTopControls(true);

            _isAuth = true;
        }

        #endregion

        private void SetNoticeMessage(TextBlock tbCoinPrice, TextBox tbCoinNoticeTarget, bool isEnableNotice, CoinList enumCoin)
        {
            if (isEnableNotice == false) return;
            if (tbCoinPrice.Text == string.Empty || tbCoinPrice.Text == "1 KRW") return;
            if (tbCoinNoticeTarget.Text.Trim() == string.Empty) return;

            string coinName = string.Empty;

            if (enumCoin == CoinList.BTC) coinName = Properties.Resources.ITEM_BTC;
            else if (enumCoin == CoinList.BCH) coinName = Properties.Resources.ITEM_BCH;
            else if (enumCoin == CoinList.ETH) coinName = Properties.Resources.ITEM_ETH;
            else if (enumCoin == CoinList.ETC) coinName = Properties.Resources.ITEM_ETC;
            else if (enumCoin == CoinList.XRP) coinName = Properties.Resources.ITEM_XRP;
            else if (enumCoin == CoinList.QTUM) coinName = Properties.Resources.ITEM_QTUM;
            else if (enumCoin == CoinList.LTC) coinName = Properties.Resources.ITEM_LTC;
            else if (enumCoin == CoinList.IOTA) coinName = Properties.Resources.ITEM_IOTA;
            else if (enumCoin == CoinList.BTG) coinName = Properties.Resources.ITEM_BTG;
            else return;
            
            long coinPrice = long.Parse(tbCoinPrice.Text.Replace("KRW", string.Empty).Trim());
            long coinNoticeTarget = long.Parse(tbCoinNoticeTarget.Text.Trim());
            
            if (coinPrice <= coinNoticeTarget)
                notifyIcon.ShowBalloonTip(100000,
                                          "Coinone Alarm",
                                          string.Format(Properties.Resources.MESSAGE_NOTICE_COIN_PRICE, coinName, coinNoticeTarget, coinPrice),
                                          System.Windows.Forms.ToolTipIcon.None);
        }
        
    }
}

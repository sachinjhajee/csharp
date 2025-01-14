using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using FiscalPrinterSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesDemo_DevGr_A
{
    public class StornoFragment : Android.Support.V4.App.Fragment
    {
        Button button;
        EditText edText;
        private View fView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public static StornoFragment NewInstance()
        {
            var detailsFrag = new StornoFragment { Arguments = new Bundle() };
            return detailsFrag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            // Use this to return your custom view for this Fragment
            fView = inflater.Inflate(Resource.Layout.Storno, container, false);

            edText = fView.FindViewById<EditText>(Resource.Id.txtOpCode);
            edText.TextChanged += TxtOpCode_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtOpPswd);
            edText.TextChanged += TxtOpPswd_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtTillNumber);
            edText.TextChanged += TxtTillNumber_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtStDocNum);
            edText.TextChanged += TxtStDocNum_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            edText.TextChanged += TxtStDocDT_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
            edText.TextChanged += TxtFMNum_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            edText.TextChanged += TxtDocUNP_TextChanged;
            edText = fView.FindViewById<EditText>(Resource.Id.txtUNP);
            edText.TextChanged += TxtUNP_TextChanged;
            button = fView.FindViewById<Button>(Resource.Id.btnStorno);
            fView.FindViewById<Button>(Resource.Id.btnStorno).Click += BtnStorno_Click;
            fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno).SetTextColor(Android.Graphics.Color.Red);
            return fView;
        }

        private void EdText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TxtOpCode_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckTextBoxes();
        }

        private void TxtOpPswd_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckTextBoxes();
        }

        private void TxtTillNumber_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckTextBoxes();
        }

        private void TxtStDocNum_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            CheckTextBoxes();
        }

        private bool IsItValidDateTime(string dt)
        {
            DateTime dateT;
            if (dt == "") return false;
            return DateTime.TryParseExact(dt, "ddMMyyHHmmss", null, System.Globalization.DateTimeStyles.None, out dateT);
        }
        private void TxtStDocDT_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var txtSt_DateTime = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            var txtFM_Num = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
            var lblCanOpenStorno = fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno);
            var btnOpen_StornoReceipt = fView.FindViewById<Button>(Resource.Id.btnStorno);
            var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            CheckTextBoxes();
            if (txtSt_DateTime.Text != "")
            {
                if (IsItValidDateTime(txtSt_DateTime.Text)) // works only for format "ddMMyyHHmmss"  ... wait for update in next version
                {
                    if (Java.Util.Regex.Pattern.Matches("DT\\d{6}-\\d{4}-\\d{7}", txtDocUNP.Text))
                    {
                        if (txtFM_Num.Text != "" && (txtFM_Num.Text.Length == 8))
                        {
                            lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Green);
                            btnOpen_StornoReceipt.Enabled = true;
                        }
                        else
                        {
                            lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                            btnOpen_StornoReceipt.Enabled = false;
                        }

                    }
                    else
                    {
                        lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                        btnOpen_StornoReceipt.Enabled = false;
                    }
                }
                else
                {
                    lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                    btnOpen_StornoReceipt.Enabled = false;
                }
                CheckTextBoxes();
            }
        }
        private void TxtFMNum_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var txtSt_DateTime = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            var txtFM_Num = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
            var lblCanOpenStorno = fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno);
            var btnOpen_StornoReceipt = fView.FindViewById<Button>(Resource.Id.btnStorno);
            var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            CheckTextBoxes();
            if (txtSt_DateTime.Text != "")
            {
                if (IsItValidDateTime(txtSt_DateTime.Text)) // works only for format "ddMMyyHHmmss"  ... wait for update in next version
                {
                    if (Java.Util.Regex.Pattern.Matches("DT\\d{6}-\\d{4}-\\d{7}", txtDocUNP.Text))
                    {
                        if (txtFM_Num.Text != "" && (txtFM_Num.Text.Length == 8))
                        {
                            lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Green);
                            btnOpen_StornoReceipt.Enabled = true;
                        }
                        else
                        {
                            lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                            btnOpen_StornoReceipt.Enabled = false;
                        }

                    }
                    else
                    {
                        lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                        btnOpen_StornoReceipt.Enabled = false;
                    }
                }
                else
                {
                    lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                    btnOpen_StornoReceipt.Enabled = false;
                }
                CheckTextBoxes();
            }
        }

        private void TxtDocUNP_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var txtSt_DateTime = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            var txtFM_Num = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
            var lblCanOpenStorno = fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno);
            var btnOpen_StornoReceipt = fView.FindViewById<Button>(Resource.Id.btnStorno);
            var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            CheckTextBoxes();
            if (txtSt_DateTime.Text != "")
            {
                if (IsItValidDateTime(txtSt_DateTime.Text)) // works only for format "ddMMyyHHmmss"  ... wait for update in next version
                {
                    if (Java.Util.Regex.Pattern.Matches("DT\\d{6}-\\d{4}-\\d{7}", txtDocUNP.Text))
                    {
                        if (txtFM_Num.Text != "" && (txtFM_Num.Text.Length == 8))
                        {
                            lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Green);
                            btnOpen_StornoReceipt.Enabled = true;
                        }
                        else
                        {
                            lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                            btnOpen_StornoReceipt.Enabled = false;
                        }

                    }
                    else
                    {
                        lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                        btnOpen_StornoReceipt.Enabled = false;
                    }
                }
                else
                {
                    lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                    btnOpen_StornoReceipt.Enabled = false;
                }
                CheckTextBoxes();
            }
        }

        private void TxtUNP_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        {
            var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            var lblCanOpenStorno = fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno);
            var btnOpen_StornoReceipt = fView.FindViewById<Button>(Resource.Id.btnStorno);
            CheckTextBoxes();
            if (txtDocUNP.Text != "")
            {
                if (Java.Util.Regex.Pattern.Matches("DT\\d{6}-\\d{4}-\\d{7}", txtDocUNP.Text))
                {
                    lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Green);
                    btnOpen_StornoReceipt.Enabled = true;
                }
                else
                {
                    lblCanOpenStorno.SetTextColor(Android.Graphics.Color.Red);
                    btnOpen_StornoReceipt.Enabled = false;
                }
            }
        }
        private bool CheckStatusErrorProperties()
        {
            string msg = "";
            try
            {
                if (MainActivity.fiscal.eSBit_GeneralError_Sharp) msg = MainActivity.fiscal.GetErrorMessage("-29");
                if (MainActivity.fiscal.eSBit_PrintingMechanism) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-28");
                if (MainActivity.fiscal.eSBit_ClockIsNotSynchronized) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-26");
                if (MainActivity.fiscal.eSBit_CommandCodeIsInvalid) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-25");
                if (MainActivity.fiscal.eSBit_SyntaxError) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-24");
                if (MainActivity.fiscal.eSBit_BuildInTaxTerminalNotResponding) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-50");
                if (MainActivity.fiscal.eSBit_LowBattery) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-56");
                if (MainActivity.fiscal.eSBit_RamReset) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-57");
                if (MainActivity.fiscal.eSBit_CommandNotPermitted) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-32");
                if (MainActivity.fiscal.eSBit_Overflow) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-33");
                if (MainActivity.fiscal.eSBit_EJIsFull) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-37");
                if (MainActivity.fiscal.eSBit_EndOfPaper) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-39");
                if (MainActivity.fiscal.eSBit_FM_NotAccess) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-46");
                if (MainActivity.fiscal.eSBit_FM_Full) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-42");
                if (MainActivity.fiscal.eSBit_GeneralError_Star) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-41");
                if (MainActivity.fiscal.eSBit_FM_ReadError) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-67");
                if (MainActivity.fiscal.eSBit_LastFMOperation_NotSuccessful) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-68");
                if (MainActivity.fiscal.eSBit_FM_ReadOnly) msg = msg + "\r\n" + MainActivity.fiscal.GetErrorMessage("-69");
                if (msg != "")
                {
                    MainActivity.ShowMessage("Status bits error(s):\r\n" + msg,"Error");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MainActivity.ShowMessage("Operation failed: " + ex.Message, "Error");
                return true;

            }
        }


        private void ExecuteFiscalCode(Action func)
        {
            var txtSt_DateTime = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            var txtFM_Num = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
             var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            var txtInvoiceNum = fView.FindViewById<EditText>(Resource.Id.txtInvoiceNum);
            var txtUNP = fView.FindViewById<EditText>(Resource.Id.txtUNP);
            var txtStDocNum = fView.FindViewById<EditText>(Resource.Id.txtStDocNum);
            var txtReason = fView.FindViewById<EditText>(Resource.Id.txtStornoReason);
            try
            {
                func();
            }
            catch (FiscalException s)
            {
                if (s.ErrorCode == -23) // error in status bytes

                    if (CheckStatusErrorProperties()) return;
                if (s.ErrorCode != 0) MainActivity.ShowMessage("Operation failed: " + s.Message,"Error");
            }
            catch (Exception ex)
            {
                //
                MainActivity.ShowMessage("Operation failed: " + ex.Message, "Error");
                //}
            }
            finally
            {
                txtDocUNP.Text = "";
                txtSt_DateTime.Text = "";
                txtStDocNum.Text = "";
                txtFM_Num.Text = "";
                txtInvoiceNum.Text = "";
                txtReason.Text = "";
                txtUNP.Text = "";
                Check_Can_OpenStornoReceipt();
                CheckStatusErrorProperties();
            }
        }

        private void Check_InvoiceRange()
        {
            var answer = MainActivity.fiscal.info_Get_InvoiceRange();

            if (answer != null && answer["valueStart"] != "" && answer["valueEnd"] != "" && answer["valueCurrent"] != "")
            {
                string startVal = answer["valueStart"];
                string endVal = answer["valueEnd"];
                string currentVal = answer["valueCurrent"];

#if DEBUGx
currentVal = "176";
#endif

                if (startVal == "0" || endVal == "0" || int.Parse(currentVal) > int.Parse(endVal))
                {
                    new Android.App.AlertDialog.Builder(MainActivity.GetInstance())
                     .SetPositiveButton("Yes", (sender, args) =>
                     {
                         ExecuteFiscalCode(() =>
                         {
                             InvoiceRangeDialog fmInvoice = new InvoiceRangeDialog(MainActivity.GetInstance());

                             fmInvoice.OnExecuteInvoice += ExecuteInvoiceRange;
                             fmInvoice.Show();

                             if (currentVal == "0") fmInvoice.startDefaultVal = "1";
                             else
                             {
                                 string currentValue = currentVal.TrimStart(new Char[] { '0' });
                                 fmInvoice.startDefaultVal = currentValue;
                             }
                         });
                     })
                     .SetNegativeButton("No", (sender, args) =>
                     {
                         return;

                     })
                     .SetMessage("You need to set proper invoice range before execute an invoice command.\r\n Do you want to set it now?")
                     .SetTitle("Warning")
                     .Show();
                }
                else
                {
                    ExecuteInvoiceRange(null, null);
                }
            }
            else return;
        }

        private int ParseCurrencyToInt(string data)
        {
            var split = data.Split(new char[] { ',', '.' });
            int value = int.Parse(split[0]);
            value *= 100;
            if (split.Length > 1)
            {
                var sub = int.Parse(split[1]);
                if (split[1].Length == 1)
                    sub *= 10;
                value += sub;
            }
            return value;
        }

        string stReasonParam = "";
        private void CalculateStorno_ReasonType()
        {
            switch (fView.FindViewById<Spinner>(Resource.Id.spinnerStornoReasonType).SelectedItemPosition)
            {
                
                case 0:
                    stReasonParam = "E"; break;
                case 1:
                    stReasonParam = "R"; break;
                case 2:
                    stReasonParam = "T"; break;
            }
        }

        private string invParam = "";
        private void SetInvoice_Value()
        {
            if (fView.FindViewById<Spinner>(Resource.Id.spinnerStornoByInvoice).SelectedItemPosition == 2) invParam = "I";
            else invParam = "";
        }

        private void ExecuteInvoiceRange(string startIntv, string endIntv)
        {
            bool isFullStorno = false;
            var txtSt_DateTime = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            var txtFM_Num = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
            var lblCanOpenStorno = fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno);
            var btnOpen_StornoReceipt = fView.FindViewById<Button>(Resource.Id.btnStorno);
            var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            var txtInvoiceNum = fView.FindViewById<EditText>(Resource.Id.txtInvoiceNum);
            var spinnerStornoReasonType = fView.FindViewById<Spinner>(Resource.Id.spinnerStornoReasonType);
            var txtOpCode = fView.FindViewById<EditText>(Resource.Id.txtOpCode);
            var txtOpPswd = fView.FindViewById<EditText>(Resource.Id.txtOpPswd);
            var txtTillNum = fView.FindViewById<EditText>(Resource.Id.txtTillNumber);
            var txtUNP = fView.FindViewById<EditText>(Resource.Id.txtUNP);
            var txtStDocNum = fView.FindViewById<EditText>(Resource.Id.txtStDocNum);
            var txtReason = fView.FindViewById<EditText>(Resource.Id.txtStornoReason);
            ExecuteFiscalCode(() =>
            {
                if (startIntv != null && endIntv != null) MainActivity.fiscal.config_Set_InvoiceRange(startIntv, endIntv);
                 var answer = MainActivity.fiscal.info_Get_CashIn_CashOut(); // Get If there is enough service money to make storno
                                                               // stornoSum = "9";
                var stornoSum = "0.01"; // Your sum for this storno receipt
                int currentAmount = 0;
                int stornoAmount = 0;
                currentAmount = ParseCurrencyToInt(answer["cashSum"]);
                stornoAmount = ParseCurrencyToInt(stornoSum);
                if (fView.FindViewById<Spinner>(Resource.Id.spinnerStornoReasonType).SelectedItemPosition != 0)
                {
                    if (currentAmount <= 0 || currentAmount < stornoAmount) // if currentAmount is less than your amount for storno to be paid (in this case 8.20)
                    {
                        MainActivity.ShowMessage("There is no enough service money for storno operation. Please, execute cash in operation!","Warning"); // use receipt_CashIn_CashOut(amount*)
                        // amount* - sum for cash in operation with + or - sign (depends on the cash in/out operation)
                        return;
                    }
                }

                CalculateStorno_ReasonType();
                SetInvoice_Value();

                MainActivity.fiscal.open_StornoReceipt(txtOpCode.Text,//
                        txtOpPswd.Text,//
                        txtTillNum.Text,//
                        invParam,//
                        txtInvoiceNum.Text,//
                        txtUNP.Text,//
                        stReasonParam,//
                        txtStDocNum.Text,//
                        txtDocUNP.Text,//
                        txtSt_DateTime.Text,//
                        txtFM_Num.Text,//
                        txtReason.Text,
                        ref isFullStorno); // this parameter returns if the command execute full storno with payment or just opens storno

                if (!isFullStorno)
                {
                    //sale item
                    MainActivity.fiscal.receipt_Sale_TextRow1("Example storno", "A", stornoSum, "1.000"); // stornoSum = amount to be paid 0.01
                    MainActivity.fiscal.receipt_Fiscal_Text(" ");
                    //total
                    MainActivity.fiscal.receipt_Total_PAmountWithoutText("P", "0.01"); // cash
                    if (fView.FindViewById<Spinner>(Resource.Id.spinnerStornoByInvoice).SelectedItemPosition == 2) // Print client info (there is and other variants. Please, check the documentation folder for more info. command 57)
                        MainActivity.fiscal.receipt_PrintClientInfo_07("#", "0000000000", "Seller name", "Receiver name", "Client name", "1234567890", " Address 1", " Address 2");
                    //close fiscal document
                    MainActivity.fiscal.receipt_Fiscal_Close();
                }
                 
                
            });
        }

        private void BtnStorno_Click(object sender, EventArgs e)
        {
            string stornoSum = "";
            bool isFullStorno = false;
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;

            var txtSt_DateTime = fView.FindViewById<EditText>(Resource.Id.txtStDocDT);
            var txtFM_Num = fView.FindViewById<EditText>(Resource.Id.txtFMNum);
            var lblCanOpenStorno = fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno);
            var btnOpen_StornoReceipt = fView.FindViewById<Button>(Resource.Id.btnStorno);
            var txtDocUNP = fView.FindViewById<EditText>(Resource.Id.txtDocUNP);
            var txtInvoiceNum = fView.FindViewById<EditText>(Resource.Id.txtInvoiceNum);
            var spinnerStornoReasonType = fView.FindViewById<Spinner>(Resource.Id.spinnerStornoReasonType);

            if (spinnerStornoReasonType.SelectedItemPosition == 1)
            {
                if (txtInvoiceNum.Text == "")
                {
                    MainActivity.ShowMessage("You must enter invoice number!","Alert");
                    return;
                }
            }
            ExecuteFiscalCode(() =>
            { 
                // if (CheckStatusErrorProperties()) return; checks error status bits
                if (MainActivity.fiscal.iSBit_Cover_IsOpen)
                {
                    MainActivity.ShowMessage(MainActivity.fiscal.GetErrorMessage("-30"), "Error");
                    return;
                }
                if (!Check_Can_OpenStornoReceipt()) return;
                if (MainActivity.fiscal.iSBit_Receipt_Fiscal)
                {
                    new Android.App.AlertDialog.Builder(MainActivity.GetInstance())
                      .SetPositiveButton("Yes", (s, args) =>
                      {
                          ExecuteFiscalCode(() =>
                          {
                              //cancel the sale
                              MainActivity.fiscal.receipt_Fiscal_Cancel();

                              Check_InvoiceRange();
                          });
                      })
                      .SetNegativeButton("No", (s, args) =>
                      {
                          MainActivity.ShowMessage("Please, pay the sum of last receipt","Alert");  // command fiscal.info_Get_FTransactionStatus; parameters: payed and amount (if payed<amount take actions for payment)
                         
                      })
                      .SetMessage("Fiscal receipt is open. Do you want to cancel it?")
                      .SetTitle("Warning")
                      .Show();

                }
                else Check_InvoiceRange();
            });
            
        }

        private void CheckTextBoxes()
        {
            if (fView == null) return;
            if (fView.FindViewById<EditText>(Resource.Id.txtOpCode).Text != "" && fView.FindViewById<EditText>(Resource.Id.txtOpPswd).Text != "" && fView.FindViewById<EditText>(Resource.Id.txtTillNumber).Text != "" && //
            fView.FindViewById<EditText>(Resource.Id.txtStDocNum).Text != "") fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno).SetTextColor(Android.Graphics.Color.Green);
            else fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno).SetTextColor(Android.Graphics.Color.Red);
            Check_Can_OpenStornoReceipt();
        }

        private bool Check_Can_OpenStornoReceipt()
        {
             bool tmpVar=false;
            bool isOpCode_Valid;
            var txtOperator_Code = fView.FindViewById<EditText>(Resource.Id.txtOpCode);
            var txtOperator_Pwd = fView.FindViewById<EditText>(Resource.Id.txtOpPswd);
            var txtTill_Num = fView.FindViewById<EditText>(Resource.Id.txtTillNumber);
            var txtSt_DocNum = fView.FindViewById<EditText>(Resource.Id.txtStDocNum);

            tmpVar = int.Parse(txtOperator_Code.Text) > 0 && int.Parse(txtOperator_Code.Text) < 17;
            tmpVar = tmpVar && ((txtOperator_Pwd.Text != "")  && (txtOperator_Pwd.Text.Length < 9));
            tmpVar = tmpVar && ((txtTill_Num.Text != "") && (txtTill_Num.Text.Length < 6));
            tmpVar = tmpVar && ((txtSt_DocNum.Text != "") && (txtSt_DocNum.Text.Length < 8));
            tmpVar = tmpVar && (!MainActivity.fiscal.iSBit_Receipt_Fiscal);
            tmpVar = tmpVar && (!MainActivity.fiscal.iSBit_Receipt_Nonfiscal);

            if (tmpVar)
            {
                fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno).SetTextColor(Android.Graphics.Color.Green);
                fView.FindViewById<Button>(Resource.Id.btnStorno).Enabled = true;
            }
            else
            {
                fView.FindViewById<TextView>(Resource.Id.lblCanOpenStorno).SetTextColor(Android.Graphics.Color.Red);
                fView.FindViewById<Button>(Resource.Id.btnStorno).Enabled = false;
            }
            
            return tmpVar;
          
        }
    }
}
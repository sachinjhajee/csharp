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
    public class ReceiptsReportsFragment : Android.Support.V4.App.Fragment
    {
        Button button;
        private View fView;
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


        }

        public static ReceiptsReportsFragment NewInstance()
        {
            var detailsFrag = new ReceiptsReportsFragment { Arguments = new Bundle() };
            return detailsFrag;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            if (container == null)
            {
                // Currently in a layout without a container, so no reason to create our view.
                return null;
            }

            // Use this to return your custom view for this Fragment
            fView = inflater.Inflate(Resource.Layout.ReceiptsReports, container, false);
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem(delegate
                {
                    var res = MainActivity.fiscal.info_Get_DiagnosticInfo();
                    this.Activity.RunOnUiThread(() =>
                    {
                        fView.FindViewById<TextView>(Resource.Id.txtSerialNum).Text = res["serialNumber"];
                    });
                }, null);
            }
            catch (FiscalException s)
            {
                if (s.ErrorCode == -23) // error in status bytes

                    CheckStatusErrorProperties();
                else MainActivity.ShowMessage("Operation failed: " + s.Message, "Error");
            }

            fView.FindViewById<TextView>(Resource.Id.txtDeviceModel).Text = MainActivity.fiscal.deviceModel;

            button = fView.FindViewById<Button>(Resource.Id.btnFiscRecUNP);
            fView.FindViewById<Button>(Resource.Id.btnFiscRecUNP).Click += BtnFiscWithUNP_Click;

            button = fView.FindViewById<Button>(Resource.Id.btnXReport);
            fView.FindViewById<Button>(Resource.Id.btnXReport).Click += BtnXReport_Click;

            button = fView.FindViewById<Button>(Resource.Id.btnFiscRec);
            fView.FindViewById<Button>(Resource.Id.btnFiscRec).Click += BtnFiscRec_Click;

            button = fView.FindViewById<Button>(Resource.Id.btnNonFiscal);
            fView.FindViewById<Button>(Resource.Id.btnNonFiscal).Click += BtnNonFiscalRec_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnCancelFiscRec);
            fView.FindViewById<Button>(Resource.Id.btnCancelFiscRec).Click += BtnCancelFiscalRec_Click;

            button = fView.FindViewById<Button>(Resource.Id.btnInvoice);
            fView.FindViewById<Button>(Resource.Id.btnInvoice).Click += BtnInvoice_Click;

            button = fView.FindViewById<Button>(Resource.Id.btnZReport);
            fView.FindViewById<Button>(Resource.Id.btnZReport).Click += BtnZReport_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnGetLastUNP);
            fView.FindViewById<Button>(Resource.Id.btnGetLastUNP).Click += BtnGetLastUNP_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnSetLanguage);
            fView.FindViewById<Button>(Resource.Id.btnSetLanguage).Click += btnSetLanguage_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnSetDT);
            fView.FindViewById<Button>(Resource.Id.btnSetDT).Click += BtnSetDT_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnGetDT);
            fView.FindViewById<Button>(Resource.Id.btnGetDT).Click += BtnGetDT_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnReportByDate);
            fView.FindViewById<Button>(Resource.Id.btnReportByDate).Click += BtnReportByDate_Click;
            button = fView.FindViewById<Button>(Resource.Id.btnReportByZNums);
            fView.FindViewById<Button>(Resource.Id.btnReportByZNums).Click += BtnReportByZNums_Click;

            if (MainActivity.fiscal != null) fView.FindViewById<Spinner>(Resource.Id.spinner).SetSelection((int)MainActivity.fiscal.language);
            var txtDisplayStDate = (TextView)fView.FindViewById(Resource.Id.txtStartDate);
            var txtDisplayEndDate = (TextView)fView.FindViewById(Resource.Id.txtEndDate);
            txtDisplayStDate.Click += MDisplayStDate_Click;
            txtDisplayEndDate.Click += TxtDisplayEndDate_Click;
            if (MainActivity.fiscal != null) fView.FindViewById<Spinner>(Resource.Id.spinnerRep).SetSelection(0);
            return fView;
        }

        DateTime startDate = DateTime.Today.AddDays(-1).Date;
        DateTime endDate = DateTime.Today.Date;

        private void MDisplayStDate_Click(object sender, EventArgs e)
        {
            DatePickerFragment fr = DatePickerFragment.NewInstance(delegate (DateTime date)
            {
                fView.FindViewById<TextView>(Resource.Id.txtStartDate).Text = "DayMonthYear: " + date.ToString("ddMMyy");
                startDate = date;
            });
            fr.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void TxtDisplayEndDate_Click(object sender, EventArgs e)
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime date)
            {
                fView.FindViewById<TextView>(Resource.Id.txtEndDate).Text = "DayMonthYear: " + date.ToString("ddMMyy");
                endDate = date;
            });

            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private bool Check_Date()
        {
            if (startDate.Date == null || endDate.Date == null) return false;

            if (startDate.Date > endDate.Date)
            {
                MainActivity.ShowMessage("Starting date is greater than ending one!", "Alert");
                //if ((dtp_StartDate.Visible) && (dtp_StartDate.Enabled)) dtp_StartDate.Focus();
                return false;
            }
            return true;
        }

        private void BtnReportByDate_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            if (fView == null) return;

                // validate date
                if (!Check_Date()) return;
                // start date in format "dd-mm-yy"
                var divider = fView.FindViewById<TextView>(Resource.Id.txtStartDate).Text.Split(": ", StringSplitOptions.None);
                string startDate = divider[1];
                // end date in format "dd-mm-yy"
                var dvd = fView.FindViewById<TextView>(Resource.Id.txtEndDate).Text.Split(": ", StringSplitOptions.None);
                string endDate = dvd[1];
            // cbRepType.SelectedIndex is used to choose between short and extended report.(0 or 1)
            ExecuteFiscalCode(() =>
            {
                if (fView.FindViewById<Spinner>(Resource.Id.spinnerRep).SelectedItemPosition == 0)
                    MainActivity.fiscal.report_FMByDateRange_Short(startDate, endDate);
                else MainActivity.fiscal.report_FMByDateRange(startDate, endDate);

            });
        }

        private void BtnReportByZNums_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            if (fView == null) return;
            if (int.Parse(fView.FindViewById<TextView>(Resource.Id.txtFromNum).Text) > int.Parse(fView.FindViewById<TextView>(Resource.Id.txtToNum).Text))
            {
                MainActivity.ShowMessage("First number of your range is greater than last one." + "\r\n" + "Please, change the values!", "Alert");
                return;
            }
            ExecuteFiscalCode(() =>
            {

                string startNum = fView.FindViewById<TextView>(Resource.Id.txtFromNum).Text; // start number of Z report
                string endNum = fView.FindViewById<TextView>(Resource.Id.txtToNum).Text;     // end number of Z report
                                                                                             // cbRepType.SelectedIndex is used to choose between short and extended report.(0 or 1)
                if (fView.FindViewById<Spinner>(Resource.Id.spinnerRep).SelectedItemPosition == 0)
                    MainActivity.fiscal.report_FMByNumRange_Short(startNum, endNum);
                else MainActivity.fiscal.report_FMByNumRange(startNum, endNum);

            });
        }

        private void BtnSetDT_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            ExecuteFiscalCode(() =>
            {

                MainActivity.fiscal.config_Set_DateTime(DateTime.Now.ToString("dd-MM-yy HH:mm:ss"));
                this.Activity.RunOnUiThread(() =>
                {
                    MainActivity.ShowMessage("Date/Time Synchronized!", "Message");
                });
            });
        }

        private void BtnGetDT_Click(object sender, EventArgs e)
        {

            if (MainActivity.fiscal == null) return;
            ExecuteFiscalCode(() =>
            {

                var dateTime = MainActivity.fiscal.info_Get_DateTime();

                StringBuilder sb = new StringBuilder("Result:\n");
                foreach (var key in dateTime.Keys)
                {
                    sb.AppendLine(key + ": " + dateTime[key]);
                }
                this.Activity.RunOnUiThread(() =>
                {
                    MainActivity.ShowMessage("Success:\n" + sb.ToString(), "Message");
                });

            });
        }

        private void btnSetLanguage_Click(object sender, EventArgs e)  // Changes language for status bytes descriotions and error messages from fiscal device
        {
            //ExecuteFiscalCode(() =>
            //{
            ManageSalesReportsControls(false);
            try
            {
                var spinner = fView.FindViewById<Spinner>(Resource.Id.spinner);
                switch (spinner.SelectedItemPosition)
                {
                    case 0:
                        {
                            MainActivity.fiscal.language = TLanguage.English;
                            spinner.SetSelection(0);
                            MainActivity.fiscal.SetStatusBits_Descriptions();
                            //get_StatusDescriptions();
                            break;
                        }
                    case 1:
                        {
                            MainActivity.fiscal.language = TLanguage.Bulgarian;
                            spinner.SetSelection(1);
                            MainActivity.fiscal.SetStatusBits_Descriptions();
                            //get_StatusDescriptions();
                            break;
                        }

                }
                MainActivity.ShowMessage("Success! " + "Current selected language is " + spinner.SelectedItem.ToString(), "Message");

            }
            catch (FiscalException s)
            {
                if (s.ErrorCode == -23) CheckStatusErrorProperties();    // error in status bytes
                else MainActivity.ShowMessage("Operation failed: " + s.Message, "Alert");

            }
            catch (Exception ex)
            {
                MainActivity.ShowMessage("Operation failed: " + ex.Message, "Alert");

            }
            finally
            {
                //CheckStatusErrorProperties();
                ManageSalesReportsControls(true);

            }


            // });
        }

        private void BtnGetLastUNP_Click(object sender, EventArgs e)
        {
            FillLastUNP();
        }

        private void GetLastUNP(ref string OpCode, ref string LastPart, ref string NextLastPart, ref string UNP)
        {
            if (fView == null) return;
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            string answer = MainActivity.fiscal.CustomCommand(48, "*"); // fiscal.CustomCommand(48,"*") - a sequential number of the sale from the Unique Sale ID
                                                                        // of the last issued fiscal receipt is returned and unp as second parameter in the answer
            string[] answ = answer.Split(',');
            if (answ.Length > 2) UNP = answ[2];
            else UNP = answ[1];
            if (string.IsNullOrEmpty(UNP))
            {
                OpCode = "0001";
                LastPart = "0000000";
                NextLastPart = "0000001";
            }
            else
            {
                var split = UNP.Split("-"); // serial number is split[0]
                OpCode = split[1];
                LastPart = split[2];
                NextLastPart = int.Parse(split[2] + 1).ToString().PadLeft(7, '0');
            }
        }

        private void FillLastUNP()
        {
            string unp = "";
            string nextLastPart = "";
            if (fView == null) return;
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            ExecuteFiscalCode(() =>
            {
               
                string answer = MainActivity.fiscal.CustomCommand(48, "*"); // fiscal.CustomCommand(48,"*") - a sequential number of the sale from the Unique Sale ID
                                                                            // of the last issued fiscal receipt is returned and unp as second parameter in the answer
                string[] answ = answer.Split(',');
                if (answ.Length > 2) unp = answ[2];
                else unp = answ[1];
                this.Activity.RunOnUiThread(() =>
                {
                    if (unp == "")
                    {
                        new Android.App.AlertDialog.Builder(MainActivity.GetInstance())
                          .SetPositiveButton("Yes", (sender, args) =>
                          {
                              ExecuteFiscalCode(() =>
                              {
                                  fView.FindViewById<Button>(Resource.Id.txtOpCode).Text = "0001";
                                  fView.FindViewById<Button>(Resource.Id.txtLastPart).Text = "0000000";
                                  fView.FindViewById<Button>(Resource.Id.txtNextLastPart).Text = "0000001";
                              });
                          })
                          .SetNegativeButton("No", (sender, args) =>
                          {
                              ManageSalesReportsControls(true);
                              return;
                          })
                          .SetMessage("There is no UNP yet in the device. Do you want the program to construct the first one for you?")
                          .SetTitle("Warning")
                          .Show();
                    }
                    else
                    {
                        string[] ans = unp.Split('-');
                        fView.FindViewById<TextView>(Resource.Id.txtSerialNum).Text = ans[0];
                        fView.FindViewById<EditText>(Resource.Id.txtOperatorCode).Text = ans[1];
                        fView.FindViewById<EditText>(Resource.Id.txtLastPart).Text = ans[2];
                        nextLastPart = (Int32.Parse(ans[2]) + 1).ToString();
                        fView.FindViewById<EditText>(Resource.Id.txtNextLastPart).Text = nextLastPart.PadLeft(7, '0');
                    }
                });

            });
        }

        private void ExecuteFiscalCode(Action func)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(delegate
            {
                this.Activity.RunOnUiThread(() => // BeginInvoke 
                {
                    ManageSalesReportsControls(false);
                });
                try
                {
                    func();
                }
                catch (FiscalException s)
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        if (s.ErrorCode == -23) CheckStatusErrorProperties();    // error in status bytes
                        else MainActivity.ShowMessage("Operation failed: " + s.Message, "Error");
                    });
                }
                catch (Exception ex)
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        MainActivity.ShowMessage("Operation failed: " + ex.Message, "Error");
                    });
                }
                finally
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        //CheckStatusErrorProperties();
                        ManageSalesReportsControls(true);
                    });
                }
            }, null);
        }

        private void BtnFiscWithUNP_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            if (fView == null) return;
            // if (CheckStatusErrorProperties()) return;
            ManageSalesReportsControls(false);
            //ExecuteFiscalCode(() =>
            //{
            //string UNP = "";
            //var OpPsw = "0000";// default password
            //var OpCode = fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text;
            //var LastPart = fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text;
            //var NextLastPart = fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text;
            //GetLastUNP(ref OpCode, ref LastPart, ref NextLastPart, ref UNP);
            //var result = MainActivity.fiscal.receipt_Fiscal_Open("1", "0000", "1", UNP);
            //return;
            try
            {
                if (MainActivity.fiscal.iSBit_Cover_IsOpen)
                {
                    MainActivity.ShowMessage(MainActivity.fiscal.GetErrorMessage("-30"), "Error");
                    return;
                }
                if (MainActivity.fiscal.iSBit_Receipt_Fiscal)
                {
                    new Android.App.AlertDialog.Builder(MainActivity.GetInstance())
                   .SetPositiveButton("Yes", (s, args) =>
                   {
                       ExecuteFiscalCode(() =>
                       {
                           //cancel the sale
                           MainActivity.fiscal.receipt_Fiscal_Cancel();
                           string UNP = "";
                           var OpPsw = "0000";// default password
                           var OpCode = fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text;
                           var LastPart = fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text;
                           var NextLastPart = fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text;
                           var SerialNum = fView.FindViewById<TextView>(Resource.Id.txtSerialNum).Text;
                           if (string.IsNullOrEmpty(OpCode) || string.IsNullOrEmpty(NextLastPart))
                           {
                               GetLastUNP(ref OpCode, ref LastPart, ref NextLastPart, ref UNP);
                           }
                           UNP = SerialNum + "-" + OpCode + "-" + NextLastPart;


                           var result = MainActivity.fiscal.receipt_Fiscal_Open("1", OpPsw, "1", UNP);
                           //print fiscal text
                           MainActivity.fiscal.receipt_Fiscal_Text("This is a demo");
                           MainActivity.fiscal.receipt_Fiscal_Text("of a fiscal text");
                           //sale item
                           MainActivity.fiscal.receipt_Sale_TextRow1("Example", "А", "0.01", "1.000");
                           //fiscal.receipt_Subtotal("1", "1");
                           //total
                           MainActivity.fiscal.receipt_Total_PAmountTextRow1("Example Total", "P", "0.01");
                           //close fiscal document
                           MainActivity.fiscal.receipt_Fiscal_Close();
                           this.Activity.RunOnUiThread(() =>
                           {
                               fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text = OpCode;
                               fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text = NextLastPart;
                               fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text = (int.Parse(NextLastPart) + 1).ToString().PadLeft(7, '0');
                           });
                       });

                   })
                   .SetNegativeButton("No", (s, args) =>
                   {

                       MainActivity.ShowMessage("Please, pay the sum of last receipt", "Alert");
                       // command fiscal.info_Get_FTransactionStatus; parameters: payed and amount (if tender<amount take actions for payment)

                       ManageSalesReportsControls(true);
                   })
                   .SetMessage("Fiscal receipt is open. Do you want to cancel it?")
                   .SetTitle("Warning")
                   .Show();
                }
                else
                {
                    ExecuteFiscalCode(() =>
                   {
                       string UNP = "";
                       var OpPsw = "0000";// default password
                       var OpCode = fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text;
                       var LastPart = fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text;
                       var NextLastPart = fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text;
                       var SerialNum = fView.FindViewById<TextView>(Resource.Id.txtSerialNum).Text;
                       if (string.IsNullOrEmpty(OpCode) || string.IsNullOrEmpty(NextLastPart))
                       {
                           GetLastUNP(ref OpCode, ref LastPart, ref NextLastPart, ref UNP);
                       }
                       UNP = SerialNum + "-" + OpCode + "-" + NextLastPart;


                       var result = MainActivity.fiscal.receipt_Fiscal_Open("1", OpPsw, "1", UNP);
                       //print fiscal text
                       MainActivity.fiscal.receipt_Fiscal_Text("This is a demo");
                       MainActivity.fiscal.receipt_Fiscal_Text("of a fiscal text");
                       //sale item
                       MainActivity.fiscal.receipt_Sale_TextRow1("Example", "А", "0.01", "1.000");
                       //fiscal.receipt_Subtotal("1", "1");
                       //total
                       MainActivity.fiscal.receipt_Total_PAmountTextRow1("Example Total", "P", "0.01");
                       //close fiscal document
                       MainActivity.fiscal.receipt_Fiscal_Close();
                       this.Activity.RunOnUiThread(() =>
                       {
                           fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text = OpCode;
                           fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text = NextLastPart;
                           fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text = (int.Parse(NextLastPart) + 1).ToString().PadLeft(7, '0');
                       });
                   });
                }
            }
            finally
            {
                ManageSalesReportsControls(true);
            }


            //});
        }

        private void BtnXReport_Click(object sender, EventArgs e)
        {

            ExecuteFiscalCode(() =>
            {
                if (MainActivity.fiscal == null) return;
                if (!MainActivity.fiscal.device_Connected) return;

                var result = MainActivity.fiscal.report_DailyClosure_01("2");

                System.Text.StringBuilder sb = new System.Text.StringBuilder("Result:\n");
                foreach (var key in result.Keys)
                {
                    sb.AppendLine(key + ": " + result[key]);
                }
                this.Activity.RunOnUiThread(() =>
                {
                    MainActivity.ShowMessage("Success:\n" + sb.ToString(), "Message");
                });
            });
        }

        private void BtnFiscRec_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            
            ManageSalesReportsControls(false);
            try
            {
                //if (CheckStatusErrorProperties()) return; // check error status bits
                if (MainActivity.fiscal.iSBit_Cover_IsOpen)
                {
                    MainActivity.ShowMessage(MainActivity.fiscal.GetErrorMessage("-30"), "Error");
                    return;
                }

                if (MainActivity.fiscal.iSBit_Receipt_Fiscal)
                {
                    new Android.App.AlertDialog.Builder(MainActivity.GetInstance())
                    .SetPositiveButton("Yes", (s, args) =>
                    {
                        ExecuteFiscalCode(() =>
                        {
                            //cancel the sale
                            MainActivity.fiscal.receipt_Fiscal_Cancel();

                            //open fiscal document
                            MainActivity.fiscal.receipt_FiscalOpen_A01("1", "0000", "1");
                            //print fiscal text
                            MainActivity.fiscal.receipt_Fiscal_Text("This is a demo");
                            MainActivity.fiscal.receipt_Fiscal_Text("of a fiscal text");
                            //sale item
                            // You can use department sale command "receipt_DSale_TextRow1" if the tax group is associated with the department when it is programmed.
                            MainActivity.fiscal.receipt_Sale_TextRow1("Example", "А", "0.01", "1.000");
                            //total
                            MainActivity.fiscal.receipt_Total_PAmountTextRow1("Example Total", "P", "0.01");
                            //close fiscal document
                            MainActivity.fiscal.receipt_Fiscal_Close();
                        });
                    })
                    .SetNegativeButton("No", (s, args) =>
                    {
                        MainActivity.ShowMessage("Please, pay the sum of last receipt", "Alert");
                        // command fiscal.info_Get_FTransactionStatus; parameters: payed and amount (if tender<amount take actions for payment)
                        ManageSalesReportsControls(true);
                    })
                    .SetMessage("Fiscal receipt is open. Do you want to cancel it?")
                    .SetTitle("Warning")
                    .Show();
                }
                else
                {
                    ExecuteFiscalCode(() =>
                    {
                        //open fiscal document
                        MainActivity.fiscal.receipt_FiscalOpen_A01("1", "0000", "1");
                        //print fiscal text
                        MainActivity.fiscal.receipt_Fiscal_Text("This is a demo");
                        MainActivity.fiscal.receipt_Fiscal_Text("of a fiscal text");
                        //sale item
                        // You can use department sale command "receipt_DSale_TextRow1" if the tax group is associated with the department when it is programmed.
                        MainActivity.fiscal.receipt_Sale_TextRow1("Example", "А", "0.01", "1.000");
                        //total
                        MainActivity.fiscal.receipt_Total_PAmountTextRow1("Example Total", "P", "0.01");
                        //close fiscal document
                        MainActivity.fiscal.receipt_Fiscal_Close();
                    });
                }
            }
            finally
            {
                ManageSalesReportsControls(true);
            }
            
        }


        private void ManageSalesReportsControls(bool value)
        {
            if (fView != null)
            {

                fView.FindViewById<Button>(Resource.Id.btnFiscRecUNP).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnFiscRec).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnCancelFiscRec).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnInvoice).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnNonFiscal).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnXReport).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnZReport).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnGetLastUNP).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnSetLanguage).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnSetDT).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnGetDT).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnReportByDate).Enabled = value;
                fView.FindViewById<Button>(Resource.Id.btnReportByZNums).Enabled = value;

            }
        }

        private void BtnNonFiscalRec_Click(object sender, EventArgs e)
        {

            ExecuteFiscalCode(() =>
            {
                if (MainActivity.fiscal == null) return;
                if (!MainActivity.fiscal.device_Connected) return;

                if (MainActivity.fiscal.iSBit_Receipt_Nonfiscal || MainActivity.fiscal.eSBit_EndOfPaper)
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        MainActivity.ShowMessage("Some of the status bits are raised!", "Warning");
                    });
                    return;
                }
                //open non-fiscal document
                MainActivity.fiscal.receipt_NonFiscal_Open();
                //print non-fiscal text
                MainActivity.fiscal.receipt_NonFiscal_Text("This is a test!");
                //print non-fiscal text in cyrillic
                MainActivity.fiscal.receipt_NonFiscal_Text("Примерен текст на кирилица");
                //close non-fiscal document
                //fiscal.receipt_Print_Barcode_02("1","","1234567");
                MainActivity.fiscal.receipt_NonFiscal_Close();

            });
        }

        private void BtnCancelFiscalRec_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            if (CheckStatusErrorProperties()) return;
            ExecuteFiscalCode(() =>
            {
                
                if (MainActivity.fiscal.iSBit_Receipt_Fiscal)
                {
                    //cancel the sale
                    MainActivity.fiscal.receipt_Fiscal_Cancel();
                }
                else
                {
                    //open fiscal document
                    MainActivity.fiscal.receipt_FiscalOpen_A01("1", "0000", "1");
                    //print fiscal text
                    MainActivity.fiscal.receipt_Fiscal_Text("This is a demo");
                    MainActivity.fiscal.receipt_Fiscal_Text("of a fiscal text");
                    //sale item
                    MainActivity.fiscal.receipt_Sale_TextRow1("Example", "А", "0.01", "1.000");
                    //cancel the sale
                    MainActivity.fiscal.receipt_Fiscal_Cancel();
                }

            });
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
                    MainActivity.ShowMessage("Status bits error(s):\r\n" + msg + "\r\n For more info check 'Status bytes' section", "Warning");
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

        private void ExecuteInvoiceRange(string startIntv, string endIntv)
        {
            var txtSerialNum = fView.FindViewById<TextView>(Resource.Id.txtSerialNum).Text;
            var txtOperatorCode = fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text;
            var txtNextLastPart = fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text;

            ExecuteFiscalCode(() =>
            {
                if (startIntv == "" && endIntv=="") MainActivity.fiscal.config_Set_InvoiceRange(startIntv, endIntv);

                if (!String.IsNullOrEmpty(txtOperatorCode) && !String.IsNullOrEmpty(txtNextLastPart))
                {
                    string unp = txtSerialNum + "-" + txtOperatorCode + "-" + txtNextLastPart;
                    // open fiscal document with UNP
                    var result = MainActivity.fiscal.receipt_Invoice_Open("1", "0000", "1", "I", unp);
                }
                else
                {
                    //open fiscal document
                    MainActivity.fiscal.receipt_Invoice_01_Open("1", "0000", "1", "I"); // third parameter must be "I" for Invoice. It autimatically print "Фактура" in the receipt
                }
                //sale item
                // You can use department sale command "receipt_DSale_TextRow1" if the tax group is associated with the department when it is programmed.
                MainActivity.fiscal.receipt_Sale_TextRow1("Example sale", "А", "0.01", "1.000");
                //print fiscal text
                MainActivity.fiscal.receipt_Fiscal_Text("----------------------------------------------");
                MainActivity.fiscal.receipt_Fiscal_Text("This is example sale for invoice");
                MainActivity.fiscal.receipt_Total_PAmountWithoutText("P", "0.01"); // cash payment
                MainActivity.fiscal.receipt_Fiscal_Text("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                MainActivity.fiscal.receipt_Fiscal_Text(" ");

                MainActivity.fiscal.receipt_PrintClientInfo_07("#", "0000000000", "Seller name", "Receiver name", "Client name", "1234567890", " Address 1", " Address 2");
                MainActivity.fiscal.receipt_Fiscal_Text("Signature");
                MainActivity.fiscal.receipt_Fiscal_Text(" ");
                MainActivity.fiscal.receipt_Fiscal_Text("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                MainActivity.fiscal.receipt_Fiscal_Close();
                //
                var txtLastPart = fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text;
                if (txtLastPart != "")
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text = fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text;
                        fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text = (Int32.Parse(fView.FindViewById<TextView>(Resource.Id.txtLastPart).Text) + 1).ToString().PadLeft(7, '0');
                    });
                }
            });
        }

        private void Check_InvoiceRange()
        {
            var txtSerialNum = fView.FindViewById<TextView>(Resource.Id.txtSerialNum).Text;
            var txtOperatorCode = fView.FindViewById<TextView>(Resource.Id.txtOperatorCode).Text;
            var txtNextLastPart = fView.FindViewById<TextView>(Resource.Id.txtNextLastPart).Text;

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

        private void BtnInvoice_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            
            try
            {
                //if (CheckStatusErrorProperties()) return;
                if (MainActivity.fiscal.iSBit_Cover_IsOpen)
                {
                    this.Activity.RunOnUiThread(() =>
                    {
                        MainActivity.ShowMessage(MainActivity.fiscal.GetErrorMessage("-30"), "Warning");
                    });
                    return;
                }

                if (fView == null) return;


                if (MainActivity.fiscal.iSBit_Receipt_Fiscal)
                {
                    this.Activity.RunOnUiThread(() =>
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
                          MainActivity.ShowMessage("Please, pay the sum of last receipt", "Alert");
                          // command fiscal.info_Get_FTransactionStatus; parameters: payed and amount (if tender<amount take actions for payment)
                          
                          ManageSalesReportsControls(true);
                      })
                      .SetMessage("Fiscal receipt is open. Do you want to cancel it?")
                      .SetTitle("Warning")
                      .Show();
                    });

                }
                else Check_InvoiceRange();
            }
            catch (FiscalException s)
            {
                if (s.ErrorCode == -23) // error in status bytes

                    if (CheckStatusErrorProperties()) return;
                if (s.ErrorCode != 0) MainActivity.ShowMessage("Operation failed: " + s.Message, "Error");
            }
            catch (Exception ex)
            {
                //
                MainActivity.ShowMessage("Operation failed: " + ex.Message, "Error");
            }
            finally
            {
                //CheckStatusErrorProperties();
                ManageSalesReportsControls(true);
            }
        }

        private void BtnZReport_Click(object sender, EventArgs e)
        {
            if (MainActivity.fiscal == null) return;
            if (!MainActivity.fiscal.device_Connected) return;
            ExecuteFiscalCode(() =>
            {
                var result = MainActivity.fiscal.report_DailyClosure_01("0");

                StringBuilder sb = new StringBuilder("Result:\n");
                foreach (var key in result.Keys)
                {
                    sb.AppendLine(key + ": " + result[key]);
                }
                this.Activity.RunOnUiThread(() =>
                {
                    MainActivity.ShowMessage("Success:\n" + sb.ToString(), "Message");
                });
            });
        }
    }
}
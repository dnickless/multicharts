Imports ATCenterProxy.interop
Imports System.Collections.Generic
Imports PowerLanguage
Imports PowerLanguage.TradeManager
Imports System
Imports System.Drawing
Imports System.Runtime.CompilerServices
Imports System.Windows.Forms

Namespace PowerLanguage.Strategy
    Public Class vb__ChartToolBar_Trading_
        Inherits SignalObject
        ' Methods
        Public Sub New(ByVal _ctx As Object)
            MyBase.New(_ctx)
        End Sub

        Private Sub _info_panel_Click(ByVal sender As Object, ByVal ev As EventArgs)
            Try 
                If (0 <> Me.m_cur_mp) Then
                    Dim orderParams As New OrderParams
                    orderParams.action = If((0 < Me.m_cur_mp), MTPA_OrdrActn.eMTPA_OA_Sell, MTPA_OrdrActn.eMTPA_OA_Buy)
                    orderParams.category = MTPA_OrdrCtgry.eMTPA_OC_Market
                    orderParams.contracts = Math.Abs(CInt(Me.m_cur_mp))
                    orderParams.tif = MTPA_OrdrTimeInForce.eMTPA_TIF_GTC
                    Me.Prof.PlaceOrder(orderParams)
                End If
            Catch e As Exception
                MessageBox.Show(e.Message)
            End Try
        End Sub

        Private Sub _poss_changed(ByVal _dummy As Position())
            Dim _poss As Position() = MyBase.TradeManager.TradingData.Positions.Items
            If (0 = _poss.Length) Then
                Me.set_panel_info(0, 0)
            Else
                Me.set_panel_info(_poss(0).Value, _poss(0).OpenPL)
            End If
        End Sub

        Private Sub _tsi_b_Click(ByVal act As MTPA_OrdrActn)
            Try
                Dim orderParams As New OrderParams
                orderParams.action = act
                orderParams.category = MTPA_OrdrCtgry.eMTPA_OC_Market
                orderParams.contracts = 1
                orderParams.tif = MTPA_OrdrTimeInForce.eMTPA_TIF_GTC
                Me.Prof.PlaceOrder(orderParams)
            Catch e As Exception
                MessageBox.Show(e.Message)
            End Try
        End Sub

        Private Sub AddItem2ToolStrip(ByVal tb As ToolStrip, ByVal item As ToolStripItem)
            item.Tag = Me
            tb.Items.Add(item)
        End Sub

        Protected Overrides Sub CalcBar()
            If MyBase.Bars.LastBarOnChart Then
                MyBase.ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(0.5))
            End If
        End Sub

        Protected Overrides Sub Destroy()
            If Me.tool_bar_inited Then
                MyBase.ChartToolBar.AccessToolBar(Sub(toolBar)
                                                      Dim forErase As New List(Of ToolStripItem)
                                                      For Each item As ToolStripItem In toolBar.Items
                                                          If ReferenceEquals(Me, item.Tag) Then
                                                              forErase.Add(item)
                                                          End If
                                                      Next
                                                      For Each item As ToolStripItem In forErase
                                                          toolBar.Items.Remove(item)
                                                      Next
                                                  End Sub)
            End If
        End Sub

        Private Shared Function GetColor(ByVal opl As Double) As Color
            Return If((0 > opl), Color.OrangeRed, Color.LawnGreen)
        End Function

        Private Shared Function MakeInfoPanelStr(ByVal mp As Integer, ByVal opl As Double) As String
            Dim _mp_string As String
            If (0 < mp) Then
                _mp_string = (mp.ToString & " Long")
                ElseIf (0 > mp) Then
                    _mp_string = (-mp.ToString & " Short")
                Else
                    _mp_string = "Flat"
                End If
            Return String.Format("{0} {1}", _mp_string, opl.ToString("C"))
        End Function

        Protected Overrides Sub OnRecalcLastBarAfterEvent()
            MyBase.TradeManager.ProcessEvents
            MyBase.ExecControl.RecalcLastBarAfter(TimeSpan.FromSeconds(0.5))
        End Sub

        Private Sub set_panel_info(ByVal mp As Integer, ByVal opl As Double)
            Me.m_cur_mp = mp
            MyBase.ChartToolBar.AccessToolBarAsync(Sub(toolBar)
                                                       m_info_panel.Enabled = 0 <> mp
                                                       m_info_panel.Text = MakeInfoPanelStr(mp, opl)
                                                       m_info_panel.BackColor = GetColor(opl)
                                                   End Sub)
        End Sub

        Protected Overrides Sub StartCalc()
            If Not Me.tool_bar_inited Then
                MyBase.ChartToolBar.AccessToolBar(
                    Sub(toolBar)
                        Dim tsiButton As New ToolStripButton
                        tsiButton.Text = "Buy Market"
                        tsiButton.BackColor = Color.DeepSkyBlue
                        tsiButton.ToolTipText = "Click for send Buy 1 Market"
                        AddHandler tsiButton.Click, Sub(_1, _2) _tsi_b_Click(MTPA_OrdrActn.eMTPA_OA_Buy)

                        AddItem2ToolStrip(toolBar, tsiButton)

                        m_info_panel = New ToolStripButton
                        m_info_panel.ToolTipText = "Click for Close Position"
                        AddHandler m_info_panel.Click, AddressOf _info_panel_Click
                        set_panel_info(0, 0)
                        AddItem2ToolStrip(toolBar, m_info_panel)

                        Dim tsiButtonS As New ToolStripButton
                        tsiButtonS.Text = "Sell Market"
                        tsiButtonS.BackColor = Color.LightCoral
                        tsiButtonS.ToolTipText = "Click for send Sell 1 Market"
                        AddHandler tsiButton.Click, Sub(_1, _2) _tsi_b_Click(MTPA_OrdrActn.eMTPA_OA_Sell)
                        AddItem2ToolStrip(toolBar, tsiButtonS)

                        AddItem2ToolStrip(toolBar, New ToolStripSeparator())
                    End Sub
                )
			Me.tool_bar_inited = True
            End If

            Me.m_prof = Nothing
            Me.m_symbol = New MCSymbolInfo(MyBase.Bars.Info.ASymbolInfo2, MyBase.Bars.Info.DataFeed)
            Dim _dummy As ITradingProfile = Me.Prof
        End Sub


        ' Properties
        Public ReadOnly Property Prof As ITradingProfile
            Get
                If (Nothing Is Me.m_prof) Then
                    Dim profile As ITradingProfile
                    For Each profile In MyBase.TradeManager.TradingProfiles
                        If (profile.Name = MyBase.Profile) Then
                            If (ETM_ConnectionChanged.eTM_CC_Connected <> profile.ConnectionState) Then
                                MyBase.ExecControl.Abort("Profile {0} is not connected!", New Object() { profile.Name })
                            End If
                            Me.m_prof = profile
                            Dim _acc As String = Me.m_prof.Accounts(0)
                            Me.m_prof.CurrentAccount = _acc
                            Me.m_prof.CurrentSymbol = Me.m_symbol
                            Dim _poss As IPositions = MyBase.TradeManager.TradingData.Positions
                            _poss.AccountFltr.CurrentValue = _acc
                            _poss.SymbolFltr.CurrentValue = Me.m_symbol.symbol.SymbolName
                            _poss.ProfileFltr.CurrentValue = MyBase.Profile
                            AddHandler _poss.Added, New TItemsChanged(Of Position)(AddressOf Me._poss_changed)
                            AddHandler _poss.Changed, New TItemsChanged(Of Position)(AddressOf Me._poss_changed)
                            AddHandler _poss.Deleted, New TItemsChanged(Of Position)(AddressOf Me._poss_changed)
                            Exit For
                        End If
                    Next
                End If
                Return Me.m_prof
            End Get
        End Property


        ' Fields
        Private m_cur_mp As Integer
        Private m_info_panel As ToolStripButton
        Private m_prof As ITradingProfile
        Private m_symbol As MCSymbolInfo
        Private tool_bar_inited As Boolean
    End Class
End Namespace

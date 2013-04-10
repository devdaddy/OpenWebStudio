'<LICENSE>
'   Open Web Studio - http://www.OpenWebStudio.com
'   Copyright (c) 2007-2008
'   by R2Integrated Inc. http://www.R2integrated.com
'      
'   Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
'   documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
'   the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
'   to permit persons to whom the Software is furnished to do so, subject to the following conditions:
'    
'   The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
'   the Software.
'   
'   This Software and associated documentation files are subject to the terms and conditions of the Open Web Studio 
'   End User License Agreement and version 2 of the GNU General Public License.
'    
'   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
'   TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
'   THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
'   CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
'   DEALINGS IN THE SOFTWARE.
'</LICENSE>
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.Plugins.Renderers

Namespace r2i.OWS.Renderers
    Public Class RenderAction
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "ACTION"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            If Not parameters Is Nothing AndAlso parameters.Length > 2 Then
                Dim valueparameter As String = parameters(1)
                Dim variableparameter As String = parameters(2)
                Dim actionparameters As String = ""
                Dim variabletypeparameter As String = "S"
                If parameters.Length > 3 AndAlso parameters(3).Length > 0 Then
                    'WE INCLUDE A TAB TARGET
                    actionparameters = parameters(3)
                End If
                '<MOD:2>
                If Not actionparameters Is Nothing AndAlso Not IsNumeric(actionparameters) Then
                    'This item was not a numeric value - which means it is either a modulesetting name, or an invalid tabid
                    'invalid TabIds are not valid, so lets assume that it is the name of a module setting
                    If Not Caller.Settings Is Nothing AndAlso Caller.Settings.ContainsKey(actionparameters) Then
                        actionparameters = Caller.Settings.Item(actionparameters)
                    Else
                        'This could be a tabname
                        'Dim tbi As DotNetNuke.Entities.Tabs.TabInfo
                        'Try
                        '    Dim tbCtrl As New DotNetNuke.Entities.Tabs.TabController
                        '    tbi = tbCtrl.GetTabByName(actionparameters, Me.PortalID)
                        'Catch ex As Exception
                        'End Try
                        'If Not tbi Is Nothing Then
                        '    actionparameters = tbi.TabID
                        'End If
                        actionparameters = AbstractFactory.Instance.EngineController.PageId(actionparameters, Caller.PortalID)
                    End If
                End If
                '</MOD:2>
                '<MOD:3>
                If parameters.Length > 4 Then
                    Select Case parameters(4).ToLower
                        Case "s", "session"
                            variabletypeparameter = "S"
                        Case "q", "querystring"
                            variabletypeparameter = "Q"
                        Case "v", "viewstate"
                            variabletypeparameter = "V"
                        Case "c", "cookie"
                            variabletypeparameter = "C"
                        Case "m", "message"
                            variabletypeparameter = "M"
                    End Select
                End If
                '</MOD:3>

                Dim pber As String = Caller.GetPostBackEventReference(valueparameter & "," & variableparameter & "," & actionparameters & "," & variabletypeparameter)
                If parameters.Length > 5 Then
                    Select Case parameters(5).ToLower
                        Case "true"
                            Source = "href=""javascript:" & pber & """"
                        Case Else
                            Source = pber
                    End Select
                Else
                    Source = "href=""javascript:" & pber & """"
                End If

                REPLACED = True
            End If
            Return REPLACED
        End Function

    End Class
End Namespace
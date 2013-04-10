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
Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Renderers

Namespace r2i.OWS.Renderers
    Public Class RenderSet
        Inherits RenderBase

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "SET"
            End Get
        End Property

        Public Overrides ReadOnly Property RenderType() As RenderTypes
            Get
                Return RenderTypes.Functional
            End Get
        End Property

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As r2i.OWS.Framework.Debugger) As Boolean
            Dim b As Boolean = False
            Dim REPLACED As Boolean = False
            Dim parameters As String() = ParameterizeString(Source, ","c, """"c, "\"c)
            Dim bSystemParse As Boolean = False
            Dim bSkipRendering As Boolean = False

            If Not parameters Is Nothing AndAlso parameters.Length > 2 Then

                'Dim name As String = parameters(1)
                'Dim value As String = parameters(2)
                'Dim location As String = parameters(3)

                'oFirewall.Clean(name)
                'oFirewall.Clean(value)
                'oFirewall.Clean(location)
                'VERSION: 2.0 - RenderString - Separate Assignment for use elsewhere.
                b = Assign(Caller, parameters(1), parameters(2), parameters(3), Source, bSystemParse)
            End If
            Return b
        End Function

        Private Function Assign(ByVal Caller As Engine, ByVal Name As String, ByVal Value As String, ByVal Location As String, ByRef Source As String, ByRef bSystemParse As Boolean) As Boolean
            'VERSION: 2.0 - RenderString - Separate Assignment for use elsewhere - NEW FUNCTION.
            Dim b As Boolean
            Select Case Location.ToUpper
                Case "MO", "MODULESETTING"
                    If Caller.Settings.ContainsKey(Name) Then
                        Caller.Settings(Name) = Value
                    Else
                        Caller.Settings.Add(Name, Value)
                    End If
                    Try
                        'Dim mc As New DotNetNuke.Entities.Modules.ModuleController
                        'AbstractFactory.Instance.EngineController.UpdateModuleSetting(Me.ModuleID, name, value)
                        Dim mdC As IModuleController = AbstractFactory.Instance.ModuleController
                        mdC.UpdateModuleSetting(Caller.ModuleID, Name, Value)
                        mdC = Nothing
                    Catch ex As Exception
                    End Try
                    Source = ""
                    b = True
                Case "SY", "SYSTEM"
                    bSystemParse = True
                Case "M", "ME", "MESSAGE"
                    'CANNOT ASSIGN TO A MESSAGE - YET.
                    b = False
                Case "S", "SESSION"
                    If Caller.Session(Name) Is Nothing Then
                        Caller.Session.Add(Name, Value)
                    Else
                        Caller.Session(Name) = Value
                    End If
                    Source = ""
                    b = True
                Case "V", "VIEWSTATE"
                    If Caller.ViewState.Item(Name) Is Nothing Then
                        Caller.ViewState.Add(Name, Value)
                    Else
                        Caller.ViewState(Name) = Value
                    End If
                    Source = ""
                    b = True
                Case "F", "FORM"
                    b = False
                Case "Q", "QUERYSTRING"
                    Caller.Response.RedirectLocation = Value
                    Source = ""
                    b = True
                Case "C", "COOKIE"
                    If Not Caller.Response.Cookies.Item(Name) Is Nothing Then
                        Caller.Response.Cookies.Item(Name).Value = Value
                    Else
                        Caller.Response.Cookies.Add(New Web.HttpCookie(Name, Value))
                    End If
                    Source = ""
                    b = True
                Case "CON", "CFG", "CONFIGURATION", "APPSETTINGS"
                    'NOT SUPPORTED
                    Source = ""
                    b = True
                Case "A", "ACTION"
                    Caller.ActionVariable(Name) = Value
                    Source = ""
                    b = True
            End Select
            Return b
        End Function
    End Class
End Namespace
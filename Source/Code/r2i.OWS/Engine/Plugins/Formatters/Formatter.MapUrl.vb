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
Imports System
Imports System.Collections
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Diagnostics
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Plugins.Formatters

Namespace r2i.OWS.Formatters
    Public Class MapUrl : Inherits FormatterBase

        Public Overrides Function Handle_Render(ByRef Caller as EngineBase, ByVal Index As Integer, ByRef Value As String, ByRef Formatter As String, ByRef Source As String, ByRef DS As System.Data.DataSet, ByRef DR As System.Data.DataRow, ByRef RuntimeMessages As System.Collections.Generic.SortedList(Of String, String), ByVal NullReturn As Boolean, ByVal NullOverride As Boolean, ByVal ProtectSession As Boolean, ByVal SessionDelimiter As String, ByVal useSessionQuotes As Boolean, ByVal useAggregations As Boolean, ByRef FilterText As String, ByRef FilterField As String, ByRef Debugger As Framework.Debugger) As Boolean
            'VERSION: 2.0 Added REVERSEPATH
            Try
                Dim Path As String
                Try
                    If Value.StartsWith("~") OrElse Value.StartsWith("/") OrElse Value.StartsWith("\") Then
                        'Path = Context.Current.Server.MapPath(Value)
                        Path = Web.HttpContext.Current.Server.MapPath(Value)
                    Else
                        Path = Value
                    End If
                Catch ex As Exception
                    'Path = Context.Current.Server.MapPath("~")
                    Path = Web.HttpContext.Current.Server.MapPath("~")
                End Try


                'Dim AppPath As String = Context.Current.Server.MapPath("~").Replace("\", "/")
                Dim AppPath As String = Web.HttpContext.Current.Server.MapPath("~").Replace("\", "/")

                Path = Path.Replace("\", "/")

                While AppPath.EndsWith("/")
                    AppPath = AppPath.Remove(AppPath.Length - 1, 1)
                End While
                While Path.EndsWith("/")
                    Path = Path.Remove(Path.Length - 1, 1)
                End While
                While AppPath.StartsWith("/")
                    AppPath = AppPath.Remove(0, 1)
                End While
                While Path.StartsWith("/")
                    Path = Path.Remove(0, 1)
                End While

                If Not Path.ToUpper.StartsWith(AppPath.ToUpper) Then
                    Source = AppPath
                Else
                    Source = String.Format("~/{0}", Path.Replace(AppPath, ""))
                End If
            Catch ex As Exception
                Source = "Unabled to reverse path"
            End Try
            Return True
        End Function

        Public Overrides ReadOnly Property RenderTag() As String
            Get
                Return "mapurl"
            End Get
        End Property
    End Class
End Namespace
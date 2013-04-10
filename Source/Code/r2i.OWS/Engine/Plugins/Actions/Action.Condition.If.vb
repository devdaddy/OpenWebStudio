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
Imports System.Xml.Serialization
Imports System.Collections.Generic
Imports System.Net.Mail
Imports System.Text
Imports r2i.OWS.Framework.Plugins.Actions
Imports r2i.OWS.Framework
Imports r2i.OWS.Framework.Utilities

Imports r2i.OWS.Framework.Entities
Imports r2i.OWS.Framework.DataAccess
Imports r2i.OWS.Framework.Utilities.Compatibility, r2i.OWS.Actions.Utilities

Namespace r2i.OWS.Actions
    Public Class ConditionIfAction
        Inherits ActionBase


#Region "Debugging and Identification: Name,Style,Description"
        Public Overrides Function Description(ByRef act As MessageActionItem) As String
            Dim str As String = ""
            If Not act.Parameters Is Nothing AndAlso act.Parameters.Count > 0 Then
                If Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_ISADVANCED_KEY)).ToUpper = "TRUE" Then
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY)))
                Else
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY)))
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_OPERATOR_KEY)))
                    str &= Utility.HTMLEncode(Utility.CNullStr(act.Parameters.Item(MessageActionsConstants.ACTIONCONDITIONIF_RIGHTCONDITION_KEY)))
                End If
            Else
                str &= " (no parameters defined)"
            End If
            Return str
        End Function
        Public Overrides Function Name() As String
            Return "If"
        End Function
        Public Overrides Function Style() As String
            Return ""
        End Function
        Public Overrides Function Title(ByRef act As MessageActionItem) As String
            Return Name()
        End Function
#End Region
        Public Overrides Function Handle_Action(ByRef Caller As RuntimeBase, ByRef sharedds As System.Data.DataSet, ByRef mi As MessageActionItem, ByRef Previous As Runtime.ActionExecutionResult, ByRef Debugger As Framework.Debugger) As Runtime.ExecutableResult
            Dim condescription As String = ""
            Dim rslt As New Runtime.ExecutableResult(Runtime.ExecutableResultEnum.Executed, "False")
            Dim passedCondition As Boolean = False
            If Not mi.Parameters Is Nothing Then 'AndAlso mi.Parameters.Count > 0 Then
                Dim sLeftHand As String = Nothing
                If mi.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY) Then
                    sLeftHand = mi.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_LEFTCONDITION_KEY)
                End If
                Dim sOperator As String = Nothing
                If mi.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_OPERATOR_KEY) Then
                    sOperator = mi.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_OPERATOR_KEY)
                End If
                Dim sRightHand As String = Nothing
                If mi.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_RIGHTCONDITION_KEY) Then
                    sRightHand = mi.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_RIGHTCONDITION_KEY)
                End If
                Dim bAdvanced As Boolean = False
                If mi.Parameters.ContainsKey(MessageActionsConstants.ACTIONCONDITIONIF_ISADVANCED_KEY) Then
                    bAdvanced = mi.Parameters(MessageActionsConstants.ACTIONCONDITIONIF_ISADVANCED_KEY)
                End If

                If bAdvanced Then
                    sOperator = ""
                    sRightHand = ""
                End If

                If Not sLeftHand Is Nothing Then    'If LeftHand isn't nothing, neither is anything else
                    Dim rm As New Renderers.RenderMath
                    If rm.EvaluateCondition(Caller.Engine, sharedds, Caller.Engine.CapturedMessages, sLeftHand, sOperator, sRightHand, condescription, Debugger) Then
                        condescription = condescription & " = True"
                        passedCondition = True
                    Else
                        condescription = condescription & " = False"
                        passedCondition = False
                    End If
                End If
            End If


            r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, condescription, True)
            If passedCondition Then
                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Passed: Handling child actions.", False)
                rslt = Caller.Execute(mi.ChildActions, Debugger, sharedds)
                If Not rslt.Result = Runtime.ExecutableResultEnum.Redirected Then
                    rslt.Result = Runtime.ExecutableResultEnum.Executed
                    rslt.Value = "true"
                End If
            Else
                r2i.OWS.Framework.Debugger.ContinueDebugMessage(Debugger, "Failed: Skipping child actions.", False)
            End If
            Return rslt
        End Function

        Public Overrides Function Key() As String
            Return "Condition-If"
        End Function
    End Class
End Namespace
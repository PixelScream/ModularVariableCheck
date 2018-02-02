using System;
using System.Collections;
using System.Reflection;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace ModularVariableCheck
{
    /// <summary>
    /// This code example creates a graph using a CodeCompileUnit and  
    /// generates source code for the graph using the CSharpCodeProvider.
    /// </summary>
    class VariableBuilder : EditorWindow 
	{
		[MenuItem ("Tools/Variable Builder")]

		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(VariableBuilder));
		}
		ModularVariableObject variableObject;
		void OnGUI () {

			variableObject = EditorGUILayout.ObjectField(variableObject, typeof(ModularVariableObject), false, null) as ModularVariableObject;

			// The actual window code goes here
			if(variableObject != null)
			{
				
				if(GUILayout.Button("Create"))
				{
					// 
					string s =  Path.GetDirectoryName(AssetDatabase.GetAssetPath(variableObject) );
					//s = Path.ChangeExtension(s, null);
					s += "/" + ClassName + ".cs";
					Debug.Log(s);
					outputFileName = s;
					Main();
					//GenerateCSharpCode(s);
					AssetDatabase.SaveAssets();
					AssetDatabase.Refresh();
				}

				if(GUILayout.Button("add to Scriptable Object"))
				{
					AddToScriptableObject();
				}
			}

			

		}

		const string 	variableName = "testAgainst",
						scriptableObjectVariablename = "scriptObjectRef";
		
    
        /// <summary>
        /// Define the compile unit to use for code generation. 
        /// </summary>
        CodeCompileUnit targetUnit;

        /// <summary>
        /// The only class in the compile unit. This class contains 2 fields,
        /// 3 properties, a constructor, an entry point, and 1 simple method. 
        /// </summary>
        CodeTypeDeclaration targetClass;

        /// <summary>
        /// The name of the file to contain the source code.
        /// </summary>
        private string outputFileName;

		private Type variableType;

        /// <summary>
        /// Define the class.
        /// </summary>
        public void Prep()
        {


            targetUnit = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace("ModularVariableCheck");
            samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));



            targetClass = new CodeTypeDeclaration(ClassName);
            targetClass.IsClass = true;
            targetClass.TypeAttributes =
                TypeAttributes.Public | TypeAttributes.Sealed;
			targetClass.BaseTypes.Add ( "ModularCheckBase" );
            samples.Types.Add(targetClass);
            targetUnit.Namespaces.Add(samples);
        }

        /// <summary>
        /// Adds two fields to the class.
        /// </summary>
        public void AddFields()
        {
			// create a reference to the scriptable object
			CodeMemberField scriptRefField = new CodeMemberField();
			scriptRefField.Attributes = MemberAttributes.Public;
			scriptRefField.Name = scriptableObjectVariablename;
			scriptRefField.Type = new CodeTypeReference(variableObject.scriptRef.GetType());
			targetClass.Members.Add(scriptRefField);

			// create the variable to check against
			CodeMemberField testAgainstField = new CodeMemberField();
			testAgainstField.Attributes = MemberAttributes.Private;
			testAgainstField.Name = variableName;

			
			if(variableType == typeof(bool))
			{
				testAgainstField.Type = new CodeTypeReference(typeof(bool));
				testAgainstField.InitExpression = 
					new CodePrimitiveExpression ( 	variableObject.compBool == 
													ModularVariableObject.ComparisonsBool.True  
					);

			}
			else
			{
				testAgainstField.Type = new CodeTypeReference(typeof(float));
				testAgainstField.InitExpression =
					new CodePrimitiveExpression (	variableObject.checkAgainstFloat );
			}
			
			targetClass.Members.Add(testAgainstField);

        }
        /// <summary>
        /// Add three properties to the class.
        /// </summary>
        public void AddProperties()
        {
			
        }

        /// <summary>
        /// Adds a method to the class. This method multiplies values stored 
        /// in both fields.
        /// </summary>
        public void AddMethod()
        {
			CodeMemberMethod checkMethod = new CodeMemberMethod();
			checkMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			checkMethod.Name = "Check";
			checkMethod.ReturnType = new CodeTypeReference(typeof(bool));

			CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement();


			
			//new CodeFieldReferenceExpression(
        	//								new CodeThisReferenceExpression(), "testAgainst");

			string compType = " == ";

			if(variableType == typeof(float))
			{
				switch(variableObject.compFloat)
				{
					case(ModularVariableObject.ComparisonsFloat.Equal):
						compType = "==";
						break;
					case(ModularVariableObject.ComparisonsFloat.Greater):
						compType = ">";
						break;
					case(ModularVariableObject.ComparisonsFloat.Less):
						compType = "<";
						break;
					case(ModularVariableObject.ComparisonsFloat.NotEqual):
						compType = "!=";
						break;

				}
			}

			returnStatement.Expression = 
					new CodeSnippetExpression(variableName + compType + 
									scriptableObjectVariablename + "." + 
									variableObject.PropName);
			
			checkMethod.Statements.Add(returnStatement);

			targetClass.Members.Add(checkMethod);


			// create the initalized function, to store a reference to the scriptable object
			CodeMemberMethod initMethod = new CodeMemberMethod();
			initMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			initMethod.Name = "Init";
			initMethod.Parameters.Add ( new CodeParameterDeclarationExpression (
				typeof(ScriptableObject), "so"
			));

			CodeFieldReferenceExpression soRef =
				new CodeFieldReferenceExpression (
					new CodeThisReferenceExpression(), scriptableObjectVariablename
				);
			initMethod.Statements.Add (
				new CodeAssignStatement( 
					soRef,
					new CodeCastExpression (
						variableObject.scriptRef.GetType(),
						new CodeArgumentReferenceExpression("so")
					)
					
				)
			);

			targetClass.Members.Add(initMethod);


        }
        /// <summary>
        /// Add a constructor to the class.
        /// </summary>
        public void AddConstructor()
        {

        }

        /// <summary>
        /// Add an entry point to the class.
        /// </summary>
        public void AddEntryPoint()
        {

        }
        /// <summary>
        /// Generate CSharp source code from the compile unit.
        /// </summary>
        /// <param name="filename">Output file name</param>

        public void GenerateCSharpCode(string fileName)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            using (StreamWriter sourceWriter = new StreamWriter(fileName))
            {
                provider.GenerateCodeFromCompileUnit(
                    targetUnit, sourceWriter, options);
            }
        }

        /// <summary>
        /// Create the CodeDOM graph and generate the code.
        /// </summary>
         void Main()
        {

			object testedVariable = variableObject.scriptRef.GetType().GetProperty(variableObject.PropName).GetValue(variableObject.scriptRef);
			variableType = testedVariable.GetType();


			Prep();
            AddFields();
            AddProperties();
            AddMethod();
        	AddConstructor();
        	AddEntryPoint();
            GenerateCSharpCode(outputFileName);

			//AddToScriptableObject();
			waitingToAddScript = true;
        }

		bool waitingToAddScript = false;

		void Update()
		{
			if(waitingToAddScript && !EditorApplication.isCompiling)
			{
				AddToScriptableObject();
				waitingToAddScript = false;
			}
		}

		public void AddToScriptableObject()
		{

			string className = ClassName;
			Debug.Log(className);
			ScriptableObject so = ScriptableObject.CreateInstance(className);
			so.name = className;
			//so.hideFlags = HideFlags.HideInHierarchy;

			AssetDatabase.AddObjectToAsset(so, variableObject);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
			if(variableObject.ModularCheck != null)
				DestroyImmediate(variableObject.ModularCheck, true);
			variableObject.ModularCheck = so as ModularCheckBase;

			(so as ModularCheckBase).Init(variableObject.scriptRef);

		}

		string ClassName 
		{
			get
			{
				string s = variableObject.name;
				s = RemoveSpaces(s) + "_MVC";
				return s;
			}
		
		}

		string RemoveSpaces(string s)
		{
			return Regex.Replace(s, @"\s+", "");
		}
    }

}
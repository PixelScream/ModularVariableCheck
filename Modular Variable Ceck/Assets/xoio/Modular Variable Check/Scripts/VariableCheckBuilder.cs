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
		VariableTransition variableObject;
		void OnGUI () {

			variableObject = EditorGUILayout.ObjectField(variableObject, typeof(VariableTransition), false, null) as VariableTransition;

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
			scriptRefField.Type = new CodeTypeReference(variableObject.tings.GetType());
			targetClass.Members.Add(scriptRefField);

			// create the variable to check against
			CodeMemberField testAgainstField = new CodeMemberField();
			testAgainstField.Attributes = MemberAttributes.Private;
			testAgainstField.Name = variableName;

			testAgainstField.Type = new CodeTypeReference(typeof(bool));

			//bool tempBool = 
			testAgainstField.InitExpression = 
				new CodePrimitiveExpression( variableObject.compBool == VariableTransition.ComparisonsBool.True  );

				//new CodeObject()
			/* 
			if(variableType == typeof(bool))
			{
				testAgainstField.Type = new CodeTypeReference(typeof(bool));
				//testAgainstField.InitExpression = new CodePrimitiveExpression(variableObject.compBool);
			}
			else
			{
				testAgainstField.Type = new CodeTypeReference(typeof(bool));
				//testAgainstField.InitExpression = new CodePrimitiveExpression(variableObject.compFloat);
				//testAgainstField.InitExpression = new CodeFieldReferenceExpression(variableObject.compFloat);
			}
			*/
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

			returnStatement.Expression = 
					new CodeSnippetExpression(variableName + " == " + 
									scriptableObjectVariablename + "." + 
									variableObject.propName);
			
			//new CodeFieldReferenceExpression(
        	//								new CodeThisReferenceExpression(), "testAgainst");

			/* 
			if(variableType == typeof(bool))
			{
				testAgainstField.Type = new CodeTypeReference(typeof(bool));
				//testAgainstField.InitExpression = new CodePrimitiveExpression(variableObject.compBool);
			}
			else
			{
				testAgainstField.Type = new CodeTypeReference(typeof(bool));
				//testAgainstField.InitExpression = new CodePrimitiveExpression(variableObject.compFloat);
			}
			*/
			checkMethod.Statements.Add(returnStatement);

			targetClass.Members.Add(checkMethod);

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

		public void AddToScriptableObject()
		{

			string className = ClassName;
			Debug.Log(className);
			ScriptableObject so = ScriptableObject.CreateInstance(className);
			so.name = className;

			AssetDatabase.AddObjectToAsset(so, variableObject);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
			
			variableObject.ModularCheck = so as ModularCheckBase;

		}

        /// <summary>
        /// Create the CodeDOM graph and generate the code.
        /// </summary>
         void Main()
        {
			Debug.Log("variable object name before " + variableObject.name);
			variableType = variableObject.prop.GetValue(variableObject.tings).GetType();
			Debug.Log("variable object name after " + variableObject.name);
            VariableBuilder sample = this;
			sample.Prep();
            sample.AddFields();
            sample.AddProperties();
            sample.AddMethod();
            sample.AddConstructor();
            sample.AddEntryPoint();
            sample.GenerateCSharpCode(outputFileName);

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

		string ClassName 
		{
			get
			{
				string s = variableObject.name;
				s = RemoveSpaces(s) + "_Class";
				return s;
			}
		
		}

		string RemoveSpaces(string s)
		{
			return Regex.Replace(s, @"\s+", "");
		}
    }

}
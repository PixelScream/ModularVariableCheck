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

namespace VariableCheck
{
    /// <summary>
    /// This code example creates a graph using a CodeCompileUnit and  
    /// generates source code for the graph using the CSharpCodeProvider.
	///
	/// TODO: support changing the name of the variable objects
	// 	TODO: an automated cooking process that scans the whole project
    /// </summary>
    class VariableBuilder : EditorWindow 
	{

		#region  EditorWindow

		[MenuItem ("Tools/Variable Builder")]
		public static void  ShowWindow () {
			EditorWindow.GetWindow(typeof(VariableBuilder));
		}

		public VariableCheckObject variableObject;
		void OnGUI () {

			variableObject = EditorGUILayout.ObjectField(variableObject, typeof(VariableCheckObject), false, null) as VariableCheckObject;

			if(variableObject != null)
			{
				if(GUILayout.Button("Cook " + variableObject.name))
				{
					Create();
				}
			}

		}

		/// <summary>
		/// sets of the cooking process for variableObject
		/// </summary>
		public void Create()
		{		
			if(!variableObject.Dirty)	
				return;

			outputFileName = Path.GetDirectoryName(AssetDatabase.GetAssetPath(variableObject) ) + "/" + ClassName + ".cs";
			Main();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
		#endregion
		
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

        /// <summary>
		/// the type of variable that's baked into the check
        /// </summary>
		private Type variableType;

        /// <summary>
        /// Define the class.
        /// </summary>
        public void DefineClass()
        {
            targetUnit = new CodeCompileUnit();
            CodeNamespace samples = new CodeNamespace("ModularVariableCheck");
            samples.Imports.Add(new CodeNamespaceImport("UnityEngine"));

            targetClass = new CodeTypeDeclaration(ClassName);
            targetClass.IsClass = true;
            targetClass.TypeAttributes =
                TypeAttributes.Public | TypeAttributes.Sealed;

			targetClass.BaseTypes.Add ( "VariableCheckBase" );
            samples.Types.Add(targetClass);
            targetUnit.Namespaces.Add(samples);
        }

        /// <summary>
        /// Create the fields for the class
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
													VariableCheckObject.ComparisonsBool.True  
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
        /// Create class's properties
        /// </summary>
        public void AddProperties()
        {
			
        }

        /// <summary>
        /// Adds a method which does a comparison between local variable
        /// and referenced variable, called Check.
		// 	Also sets up Init method
        /// </summary>
        public void AddMethod()
        {
			// Create Check method
			CodeMemberMethod checkMethod = new CodeMemberMethod();
			checkMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			checkMethod.Name = "Check";
			checkMethod.ReturnType = new CodeTypeReference(typeof(bool));

			// Comparison function 
			string compType = " == ";
			if(variableType == typeof(float))
			{
				switch(variableObject.compFloat)
				{
					case(VariableCheckObject.ComparisonsFloat.Equal):
						compType = " == ";
						break;
					case(VariableCheckObject.ComparisonsFloat.Greater):
						compType = " < ";
						break;
					case(VariableCheckObject.ComparisonsFloat.Less):
						compType = " > ";
						break;
					case(VariableCheckObject.ComparisonsFloat.NotEqual):
						compType = " != ";
						break;

				}
			}

			// Creates a return statment which compares local and ref variable
			CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement();
			returnStatement.Expression = 
					new CodeSnippetExpression(variableName + compType + 
									scriptableObjectVariablename + "." + 
									variableObject.PropName);
			checkMethod.Statements.Add(returnStatement);

			// Adds Check method to class
			targetClass.Members.Add(checkMethod);




			// Create the Initalized function, 
			// to store a reference to the scriptable object
			CodeMemberMethod initMethod = new CodeMemberMethod();
			initMethod.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			initMethod.Name = "Init";

			// Incoming Scriptable Object parameter 'so'
			initMethod.Parameters.Add ( new CodeParameterDeclarationExpression (
				typeof(ScriptableObject), "so"
			));

			// Adds lines which casts income SO to correct type
			// then assigns it to local variable
			initMethod.Statements.Add (
				new CodeAssignStatement( 
					new CodeFieldReferenceExpression (
						new CodeThisReferenceExpression(), scriptableObjectVariablename
					),
					new CodeCastExpression (
						variableObject.scriptRef.GetType(),
						new CodeArgumentReferenceExpression("so")
					)
					
				)
			);

			targetClass.Members.Add(initMethod);


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
			// Uses relection to get the type of variable being checked against
			variableType = variableObject.scriptRef.GetType()
							.GetProperty(variableObject.PropName)
							.GetValue(variableObject.scriptRef).GetType();


			DefineClass();
            AddFields();
            AddProperties();
            AddMethod();
            GenerateCSharpCode(outputFileName);

			//AddToScriptableObject();
			waitingToAddScript = true;
        }


		/// <summary>
        /// Need to wait for Compiler to finish,
		/// then see if any VariableObjects need updating
		/// (Might be a better way of looking for this calback)
        /// </summary>
		bool waitingToAddScript = false;
		void Update()
		{
			if(waitingToAddScript && !EditorApplication.isCompiling)
			{
				AddToScriptableObject();
				waitingToAddScript = false;
			}
		}

		/// <summary>
        /// If VariableObject doesn't have a child create one
		/// TODO: make this more robust system for checking for changes + updating,
		/// and do things like support renaming + other edgecases
        /// </summary>
		public void AddToScriptableObject()
		{
			//Debug.Log("modular check = " + variableObject.ModularCheck);
			if(variableObject.ModularCheck == null )
			{
				string className = ClassName;
				//Debug.Log(className);
				ScriptableObject so = ScriptableObject.CreateInstance(className);
				so.name = className;
				//so.hideFlags = HideFlags.HideInHierarchy;

				AssetDatabase.AddObjectToAsset(so, variableObject);

				/* (doesn't work)
				if(variableObject.ModularCheck != null)
					DestroyImmediate(variableObject.ModularCheck, true);
				
				*/
				variableObject.ModularCheck = so as VariableCheckBase;

				(so as VariableCheckBase).Init(variableObject.scriptRef);

			}

			variableObject.Dirty = false;

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Resources.UnloadAsset(variableObject.ModularCheck);

			//Resources.Load( AssetDatabase.GetAssetPath(variableObject)); 
			//Selection.activeObject = variableObject;
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
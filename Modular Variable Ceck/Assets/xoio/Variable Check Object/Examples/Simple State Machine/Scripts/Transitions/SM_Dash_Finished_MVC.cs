// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace ModularVariableCheck
{
    using UnityEngine;
    
    
    public sealed class SM_Dash_Finished_MVC : VariableCheckBase
    {
        
        public StateDash scriptObjectRef;
        
        private bool testAgainst = false;
        
        public override bool Check()
        {
            return testAgainst == scriptObjectRef.Dashing;
        }
        
        public override void Init(UnityEngine.ScriptableObject so)
        {
            this.scriptObjectRef = ((StateDash)(so));
        }
    }
}
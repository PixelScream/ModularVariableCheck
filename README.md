# ModularVariableCheck
Proof of concept for a modular/component based variable check system.<br />
By combining this with a event/subscriber pattern you can put the power in the hands of the designers and remove the need for hard coded checks.<br />
<br />
Works with any Scriptable Object with the aim to avoid circular dependencies and maximum modularity<br />

# Usage
- Create a new Modular Variable Object Assets>Create>xoio>Variable>Variable Object
- Assign the new Variable Object's 'script ref' and then select the conditions you want to be checked against
(It will check against any public property of the supported types)
- Now hitting 'Cook' will create a build ready version or use as is for faster testing and iteration

- Inside a script you want to use this system with simply make a reference to a public Modular Variable Object
- And check against ModularVariableObject.Check
- In the inspector assign the desired Modular Variable Object to the component and you're all done

# Future developments
This is a just a simple implementation of the idea.
- A full version would include support for more data types than just bool and float
- A project wide baker that would automate baking + check at build time

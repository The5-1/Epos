/*******************************
Events / Delegates

Sigleton (avoid this usually use events)

Stateless vs Statefull (try to separate strictly)

Handler: takes a object, modifies it, passes ist along, stateless



Unity Intermediate Scripting
https://unity3d.com/de/learn/tutorials/topics/scripting/properties?playlist=17117


Listener: the object that registerd a method to the delegate
Callback: the thing that notifies the listener, here the delegate or event itself
Event Trigger: e.g. Setter: The setter itself is called by some piece of code too, so there is actually no constant querrying!!!

float health {
get {return _health;} 
set  { _health = value;  OnDamage.Invoke(value); }


Coroutines = efficient alternative to Update()
- e.g. for things that Lerp rotation, custom animation system or enviromnment animations like wind?
--> Have a loop run that pauses at yield and checks if something has changed.
--> run a coroutine in the setter of a property!
--> e.g. move to where you clicked without Update()
--> exits loop when you reached destinations
--> generally to avoid Update() because coroutines are cheaper
--> eg make the actor text floater turn to the camera in coroutines --> camera position change sets "target" property on floater (event?)

Delegates:
--> add multiple functions to a variable
    delegate void MyDelegate(int num); = we take functions of form "void f(int)"
    void funcA(int num);
    void funcB(int num);
    MyDelegate delegate; delegate += funcA(1); delegate += funcB(5); <-- multicasting the delegate with multiple functions
--> delegate needs a method, if its null it causes a error, same for events

Events: 
= work similar like public multicast delegates  /  check observer pattern
--> The Event Manager does not need to know about any methods of other classes
--> Call those when properties change?
+ Event Manager
--> the event manager creates a delegate that methods with the same pattern can be added to
--> the event manager also creates a STATIC event (so it can be used without a class instance)
--> the PUBLIC STATIC EVENT is of the type of the DELEGATE!!!!
--> the event manager then calls the event like a method, when the event is triggered.
+ Subscriber
--> OnEnable(): += use the PUBLIC STATIC EVENT of the event manager to register their method to it
--> OnDisable(): -= remove method from the PUBLIC STATIC EVENT (important)
+ is technocally SAME as normal public DELEGATE
--> but EVENTS only allow += or -=, not overwriting or deleting, so it is more secure
+ Problem: non MonoBehavior have none of the Enable and Disable stuff...


Property:
--> class variable with get{} set{}

Attributes = prefixes in square brackets
--> [Range(-100,100)] --> gives a slider in edit mode
--> [ExecuteInEditMode] --> Script applies in edit mode --> permanent changes! --> editor scripts

Callback:
???
https://docs.unity3d.com/ScriptReference/Camera.CameraCallback.html
http://answers.unity3d.com/questions/59983/callback-on-variable-change-inside-editor.html

Collections:
--> List can be sorted but the class needs to inherit from IComparable (in our case possibly compare region ID?)
--> Dictionary is to acces by key.

Generic <T> = no need for inheritance
--> Use different objects that do not inherit from the same base class


Extension Methods = add stuff to class with new class
--> Add a Helper method to a existing class without modifying the class?





********************************/
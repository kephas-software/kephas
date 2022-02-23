#r "System.Dynamic"
#r "Microsoft.CSharp"
int Power(int a) => a * a;
dynamic dargs = globals.Args;
return Power(dargs.a);
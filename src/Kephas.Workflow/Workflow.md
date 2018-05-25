# Workflow

## Activity

Instance of an activity which can be executed. Contains a list of parameters/is an expando.

## ActivityHandler

Executes an activity instance and returns the output/ref parameters.

### Execute/Async
TODO: sets the output paramters into the activity instance, or returns them in a new structure?
activity instance:
+ can be seen as an execution state
- not very functional programming like

## State machines

### State

initial, final

### Transition
A transition is a special kind of activity which moves the state machine from the current state to another one.

#### From
Match criteria for source states - final states are ignored

#### To
Match criteria for target states

####
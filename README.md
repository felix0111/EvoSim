# EvolutionSimulator2D
 
This simulator tries to simulate the evolution of agents (called "Entity") and their brains. 

## Neural Network
The each entity is assigned a neural network, its brain. The neural network and its methods are based on NEAT (NeuroEvolution of Augmenting Topologies). See also my project NeuraSuite. 

### The mutations that can happen to a neural network
-add connection  
-remove connection  
-adjust weight  
-toggle connection  
-add neuron  
-remove neuron  
-change activation function of a random neuron randomly  

### The input neurons of an entities neural network
-StomachFullness  
-Energy  
-Health  
-Age  
-IsColliding  
-IsPregnant  
-AnglePlants  
-AngleMeats  
-AngleEntities  
-DistancePlants  
-DistanceMeats  
-DistanceEntities  
-SameSpeciesInView  
-PheromoneR  
-PheromoneG  
-PheromoneB  
-PheromoneAngle  
-Bias  
-Random  
-Oscillator  

### The output neurons of an entities neural network
-ActionAttack  
-ActionEat  
-ActionMoveForward  
-ActionMoveRight  
-ActionRotate  
-ActionReproduce  
-ActionVisionAngle  
-ActionDigest  
-ActionPheromone  

## How the simulation works
At start a certain amount of entities are spawned. Each entity has its own neural network (brain). 

### World
All entities are spawned in a big food area. The food area fills itself with food (plants) periodically up until a maximum amount is reached. 

### Interacting with the world
All entities must eat food so that they dont starve. Entities can either be herbivore or carnivore and thus only digest plants or meats. 

### Vision system
An entity has a vision cone with a specific length and field of view. The vision cone can be rotated to the left and right independent of its body rotation. The neural network (brain) can dicern between plant, meat, entity and also get information about its distance and general angle of each type seperately.

### Fighting
An entity can attack another entity and reduce its health. When an entity dies of hunger or damage, it drops food (meat). 

### Reproduction
Entities can get pregnant which takes a certain amount of time and also increases their hunger drain. A pregnancy starts when their neural network activates the "ActionReproduce" output neuron. Reproduction can happen asexual (happens most of the time) and sexual (only when they touch another entity at the moment a pregnancy is started (unlikely to happen). When pregnancy is started, the entity will consume and accumulate extra energy until a certain threshold is reached. The pregnancy is then finished and a new entity will spawn with a copy of the neural network of its parent. The neural network of the child has a certain chance to get mutated when it is first spawned.

### Pheromones
An entity can emit pheromones. A pheromone has a certain color (RGB) which is used to dicern pheromones from different entities. A pheromone also has an angle which tells the angle in which the emitter moved. The color of the pheromone an entity emits stays the same for its whole lifetime but may change in its child randomly by a small amount. This makes it possible for an entity to detect if an entity from another niche/species was recently at a specific location and in which direction it moved.

### Genome
Each entity has a genome which consists of the following genes:  
-entity size  
-entity color  
-pheromone color  
-maximum speed  
-maximum vision distance  
-field of view  
-the frequency of the oscillator input neuron  
-diet (herbivore or carnivore)

Some genes like entity size also have an effect on stomach size, hunger drain, digestion rate, health, attack damage. A gene may change randomly with mutation (when a child is created).

### Some general information about the current state of the simulation
The diet system allows for a predator-prey model but it happens only very rarely. This is probably because at the start, only harbivores are spawned as the initial population. Also the chances for the diet gene to mutate to "carnivore" AND the mutation of the correct neural network connections at the same time are very low. Spawning carnivores with a manually engineered neural network in a herbivore dominated simulation can force a predator-prey model.  
Entities sometimes evolve to be bigger in size so that they have more health, can store more food in their stomach and eat more food at once.

Most of the time the simulation leads to the following behaviour of the entities:  
-looking left and right periodically to search for food  
-rotate in the direction of the food and move towards it  
-move left, right or backwards when entity in sight  

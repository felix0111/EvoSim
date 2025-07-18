# EvoSim

<img src="https://github.com/user-attachments/assets/38554212-24a3-42a0-ad20-6e1c48c92200" width="1000">

This simulator tries to simulate the evolution of agents (called "Entity") and their brains (neural networks). 

## Neural Network
<img src="https://github.com/user-attachments/assets/e6b49b80-3bb8-40bd-96fb-03471a55ddfb" width="800">

Each entity is assigned a neural network, its brain. The neural network and its methods are based on NEAT (NeuroEvolution of Augmenting Topologies). See also [NeuraSuite](https://github.com/felix0111/NeuraSuite)  
Evolving a neural network with the NEAT method allows for dynamic and structural mutation, speciation and recurrent connections for LSTM-like behaviour.

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
-ActionPheromone  

### The mutations that can happen to a neural network
-add connection  
-remove connection  
-adjust weight  
-toggle connection  
-add neuron  
-remove neuron  
-change activation function of a random neuron randomly  

## Simulation

### World
The world consist of so called food areas. These circular food areas fill themself with food (plants) periodically up until a maximum amount is reached. At the start, initial entities are spawned randomly throughout these areas.

### Food
Food can be either plant or meat. Each food has a certain nutritional value which also has an influence on its size. Foods decompose after a long enough time. This is done to reduce accumulation of food in hard to accessible areas.

## Entity

### Health
The health of an entity is dependent on its body size. The bigger the entity, the more health.

### Energy
All entities can store a specific amount of energy depending on the body size. The rate at which the energy of an entity drains depends on its size, health, current movement speed and other things. Entities must eat food so that they dont starve. When their energy is completely used up, they die instantly.

### Eating
An entity can eat food in front of them by activating its "ActionEat" output neuron. Eating has a certain cooldown and a limit to how much the entity can intake at once. This means a food is not instantly consumed but in smaller steps. Bigger entities can intake more food at once.

### Digestion
Each entity has a stomach size depending on its body size. When food is in an entities stomach, it gets digested. The digestion rate is controlled by a gene and can change by mutation. Digesting food fills the entity's energy storage.

### Diet
Entities can be either herbivore or carnivore and thus only digest plants or meats. Digesting meat gives more energy per amount of food than plant. This should give carnivores a small advantage.

### Vision system
An entity has a vision cone with a specific length and field of view. The vision cone can be rotated to the left and right independent of its body rotation. The neural network (brain) can dicern between plant, meat, entity and also get information about its distance and general angle of each type seperately.

### Fighting
An entity can attack another entity and reduce its health. When an entity has zero health, it dies and drops meat.

### Pregnancy
Entities can get pregnant which takes a certain amount of time and energy. A pregnancy starts when their neural network activates the "ActionReproduce" output neuron. Reproduction can happen asexual (happens most of the time) and sexual (only when they touch another entity at the moment a pregnancy is started (unlikely to happen). When pregnancy is started, the entity will consume and accumulate extra energy until a certain threshold is reached. The time and amount of energy that is needed for creating a child is controlled by two genes. When the pregnancy is finished, a new entity will spawn with an initial energy that corresponds to the parents invested energy. The neural network of a child is either a copy of its parent (asexual reproduction) or a crossover of both parents (sexual reproduction). The neural network of the child also has a random chance to get mutated when it is first spawned.  

### Speciation
At the start of an entity's lifetime it is assigned to a species similar to the traditional NEAT implementation. When an entity differs to much from every species representative, a new species will be created. The neural network of an entity also gets information about whether an entity in view is in the same species or not.

### Species Member Budget (optional)
A member budget which is used to limit the size of a species is calculated for each species periodically. This is enforced by preventing pregnancy of a member when the member count is higher than the budget. The member budget of each species is proportional to the sum of adjusted fitnesses of its members and thus preventing large species to dominate the simulation. Limiting species by a budget is optional and disabled by default as it may interfere with natural evolution too much.

### Pheromones
An entity can emit pheromones. A pheromone has a certain color (RGB) which is used to dicern pheromones from different entities. A pheromone also stores information about the direction the emitter (entity) moved. The color of the pheromone an entity emits stays the same for its whole lifetime but may change in its child randomly by mutation. Pheromones decay over time.

### Smelling
The neural network of an entity can get information of the red, green, and blue values of a pheromone and the direction of the emitter in form of an angle. This makes it possible for an entity to detect if an entity from another niche/species was recently at a specific location and in which direction it moved.

### Genome
Each entity has a genome which consists of the following genes:  
-entity size  
-entity color  
-pheromone color  
-maximum speed  
-maximum vision distance  
-field of view  
-the frequency of the oscillator input neuron  
-pregnancy time
-pregnancy energy invest
-diet (herbivore or carnivore)

A gene may change randomly with mutation (when a child is created).

### Entity Size
The entity size gene has an effect on the following features:
-movement speed
-total energy storage
-food intake per bite
-stomach size
-digestion speed
-constant energy drain
-energy drain when moving 
-health
-attack damage
-defense

### Fitness
Each entity has a certain fitness value at all times. The fitness basically represents the current performance of an entity. It is influenced by the entity's damage dealt and taken, children count and average energy. The fitness is only relevant when the species member budget is enabled.

## Some general information about the current state of the simulation
The diet system allows for a predator-prey model but it happens only very rarely. This is probably because at the start, only harbivores are spawned as the initial population. Also the chances for the diet gene to mutate to "carnivore" AND the mutation of the correct neural network connections at the same time are very low. Spawning carnivores with a manually engineered neural network in a herbivore dominated simulation can force a predator-prey model.  

Most of the time the simulation leads to the following behaviour of the entities:  
-rotating randomly to search for food  
-when food is in view, rotate in the direction of the food and move towards it  
-move left, right or backwards when an entity is in sight  

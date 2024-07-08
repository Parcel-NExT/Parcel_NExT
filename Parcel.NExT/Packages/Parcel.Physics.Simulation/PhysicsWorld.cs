using Parcel.Simulation;

namespace Parcel.Physics.Simulation
{
    public class PhysicsWorld
    {
        public PhysicsWorld(PhysicsWorldConfiguration options)
        {
            Options = options;
        }
        #region Properties
        public double CurrentSimulationTime { get; private set; }
        public PhysicsWorldConfiguration Options { get; }
        #endregion

        #region Methods
        public void AddBody(PhysicsWorld world)
        {

        }
        public void Simulate(double timeStep, double duration)
        {

        }
        #endregion
    }
}

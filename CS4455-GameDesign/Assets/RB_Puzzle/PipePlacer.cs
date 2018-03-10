using System.Globalization;
using System.Security.Policy;
using UnityEngine;

namespace RB_Puzzle
{
	public class PipePlacer : MonoBehaviour
	{
		private const int NumPipeVarieties = 2;
		
		public Transform[] PipeVarieties = new Transform[NumPipeVarieties];
		
		// Use this for initialization
		void Start ()
		{
			GeneratePipeGrid(20, 30);
		}

		void GeneratePipeGrid(int numWide, int numLong)
		{
			//params width and height refer to the number of pipes
			const float spacing = 1f;
			Vector3 center = new Vector3(0, 0, 25);
			Vector3 bottomLeftPos = center - new Vector3((spacing * numWide)/2f, 0, (spacing * numLong)/2f);
			
			for (int i = 0; i < numWide; i++) {
				for (int j = 0; j < numLong; j++) {
					//choose random pipe
					int pipeIndex = Random.Range(0, NumPipeVarieties);
					float xPos = bottomLeftPos.x + i * spacing;
					float zPos = bottomLeftPos.z + j * spacing;
					float yRot = GetRandomRotation();
					PlacePipe(pipeIndex, xPos, zPos, 0, yRot);
				}
			}
		}
		
		

		void PlacePipe(int pipeIndex, float xPos, float zPos, float xRot, float yRot)
		{
			const float y = 2.5f;

			Transform pipe = PipeVarieties[pipeIndex];
			
			Vector3 position = new Vector3(xPos, y, zPos);
			
			//Quaternion rotation = Quaternion.identity;
			Quaternion rotation = Quaternion.Euler(new Vector3(xRot, yRot, 0));
			
			Instantiate(pipe, position, rotation);
		}

		float GetRandomRotation()
		{
			int rotChoice = Random.Range(0, 4);
			return rotChoice * 90;
		}
		
		
	}
}

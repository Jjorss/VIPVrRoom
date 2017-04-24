using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class Movement : MonoBehaviour {
    public float speed;
    public float move; 

    private string port;
    private bool calibrating = false;
    private int cCount = 0;
    private Rigidbody rb;
    SerialPort sp;//= new SerialPort("COM3",115200);

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        print("Starting");
        print("Pinging all ports for glove");
        foreach (string s in SerialPort.GetPortNames())
        {
            sp = new SerialPort(s, 115200);
            sp.Open();
            sp.ReadTimeout = 10;
            sp.Write("P\r");
            if (sp.ReadLine() == "PONG")
            {
                port = s;
                sp.Close();
                break;
            }
            sp.Close();
        }
        print("Found glove on port " + port);
        sp = new SerialPort(port, 115200);
        sp.Open();
        sp.ReadTimeout = 10;
        //Calibrate ();
        print(sp.IsOpen + " : State of COM port");
        //  sp.ReadBufferSize=32;
    }

    void FixedUpdate()
    {
        //move = speed * Time.deltaTime;
        float moveHorizontal = Input.GetAxis("Horizontal");
        //MoveObject (moveHorizontal);
        if (moveHorizontal < 0 && !calibrating)
        {
            sp.Write("C\r");
            calibrating = true;
            print("Calibrating");
        }
        if (moveHorizontal > 0 && calibrating)
        {
            if (cCount < 2)
            {
                sp.Write("F\r");
                System.Threading.Thread.Sleep(2000);
                cCount++;
                print(cCount);
            }

        }
        if (cCount == 2)
        {
            calibrating = false;
            cCount = 0;
        }
        if (sp.IsOpen && !calibrating)
        {
            //print("Start");
            try
            {
                sp.Write("D\r");

                var tokens = new List<int>();
                int pos = 0;
                int start = 0;
                string s = sp.ReadLine();
                //print(s);
                do
                {
                    pos = s.IndexOf(',', start);

                    if (pos >= 0)
                    {
                        // print(s.Substring(start, pos - start).Trim()); 
                        tokens.Add(System.Convert.ToInt32(s.Substring(start, pos - start).Trim()));
                        start = pos + 1;


                    }

                } while (pos > 0);

                MoveObject(tokens[0], tokens[1]);
            }
            catch (System.Exception e)
            {
                print("Error when reading data " + e);
            }
        }

    }

    void MoveObject(float x, float y)
    {
        speed = 10;
        if (x < 0)
        {
            x = 0;
        }
        if (x > 90)
        {
            x = 90;
        }
        if (y < 0)
        {
            y = 0;
        }
        if (y > 90)
        {
            y = 90;
        }
        print(x + " : " + y);

        if (x < 30)
        {
            Vector3 movement = new Vector3((0 - x) / 45, 0.0f, 0);
            rb.AddForce(-1 * movement * speed);
        }
        if (x > 60)
        {
            Vector3 movement = new Vector3((x - 45) / 45, 0.0f, 0);
            rb.AddForce(-1 * movement * speed);
        }
        if (y < 30)
        {
            Vector3 movement = new Vector3(0, 0, (0 - y) / 45);
            rb.AddForce(movement * speed);
        }
        if (y > 60)
        {
            Vector3 movement = new Vector3(0, 0, (y - 45) / 45);
            rb.AddForce(movement * speed);
        }
    }

    void Calibrate()
    {
        print("HELO");
        /*if (sp.IsOpen) {
			float moveHorizontal = 0;

			do {
				moveHorizontal =	Input.GetAxis ("Horizontal");
			} while (moveHorizontal >= 0);
			print ("Here1");
			try {
				if (moveHorizontal < 0) {
					sp.Write ("C/n");
				}
				moveHorizontal = 0;
				do {
					moveHorizontal =	Input.GetAxis ("Horizontal");
				} while (moveHorizontal <= 0);
				sp.Write ("F/n");
				moveHorizontal = 0;
				do {
					moveHorizontal =	Input.GetAxis ("Horizontal");
				} while (moveHorizontal <= 0);
				sp.Write ("F/n");

			} catch (System.Exception) {
			}
				
		}
        */
    }
}

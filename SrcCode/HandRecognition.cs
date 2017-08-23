//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/21
// Description: hand recognition interface for palm or fist recognition
// version: v0.2 
// type: interface
//////////////////////////////////////////////////////////////////////////



namespace FesianXu.KinectGestureControl
{

    using TensorFlow;

    enum HandsTypeEnum { LeftPalm, LeftFist, RightPalm, RightFist};
    interface HandRecognition
    {
        /// <summary>
        /// judge the hands type
        /// </summary>
        /// <param name="img">hands roi color image</param>
        /// <returns>the type of hands</returns>
        HandsTypeEnum judgeHandsType(ref byte[] img, HandsEnum whichHand);
    }

    partial class MainWindow
    {
        private bool isLhandFirstToCNN = true;
        private bool isRhandFirstToCNN = true;

        private float[] recognizeHands(HandsEnum whichHand, ref byte[] hand_c3)
        {
            if (whichHand == HandsEnum.leftHand)
            {
                var lhand_runner = lhand_global_sess.GetRunner();
                var lhand_tensor_g = new TFTensor(hand_c3);
                var lhand_tensor_m = new TFTensor[] { lhand_tensor_g };    
                using (TFGraph local_graph = new TFGraph())
                {
                    lhand_tensor = CreateTensorFromRawTensor(ref lhand_tensor_m, local_graph);
                }
                lhand_runner.AddInput(lhand_global_graph["input"][0], lhand_tensor).
                           AddInput(lhand_global_graph["dropout"][0], 1.0f).
                           Fetch(lhand_global_graph["softmax_output"][0]);  
                TFTensor[] lhand_output;    
                if (isLhandFirstToCNN)
                {
                    this.sensor.AllFramesReady -= this.AllFrameReadyHandle;
                    lhand_output = lhand_runner.Run();
                    this.sensor.AllFramesReady += this.AllFrameReadyHandle;
                    isLhandFirstToCNN = false;
                }
                else
                {
                    lhand_output = lhand_runner.Run();
                }
                var result_l = lhand_output[0];
                var p = ((float[][])result_l.GetValue(jagged: true))[0];
                //release and dispose
                lhand_tensor.Dispose(true);
                result_l.Dispose(true);
                for (int ind = 0; ind < lhand_output.Length; ind++)
                    lhand_output[ind].Dispose(true);
                return p;
            }
            else if (whichHand == HandsEnum.rightHand)
            {
                var rhand_runner = rhand_global_sess.GetRunner();
                var rhand_tensor_g = new TFTensor(hand_c3);
                var rhand_tensor_m = new TFTensor[] { rhand_tensor_g };
                using (TFGraph local_graph = new TFGraph())
                {
                    rhand_tensor = CreateTensorFromRawTensor(ref rhand_tensor_m, local_graph);
                }
                rhand_runner.AddInput(rhand_global_graph["input"][0], rhand_tensor).
                           AddInput(rhand_global_graph["dropout"][0], 1.0f).
                           Fetch(rhand_global_graph["softmax_output"][0]);
                TFTensor[] rhand_output;
                if (isRhandFirstToCNN)
                {
                    this.sensor.AllFramesReady -= this.AllFrameReadyHandle;
                    rhand_output = rhand_runner.Run();
                    this.sensor.AllFramesReady += this.AllFrameReadyHandle;
                    isRhandFirstToCNN = false;
                }
                else
                {
                    rhand_output = rhand_runner.Run();
                }
                var result_r = rhand_output[0];
                var p = ((float[][])result_r.GetValue(jagged: true))[0];

                //release and dispose
                rhand_tensor.Dispose(true);
                result_r.Dispose(true);
                for(int ind = 0; ind < rhand_output.Length; ind++)
                    rhand_output[ind].Dispose(true);
                return p;
            }
            return new float[] { 0, 0 };

        }


        private static TFTensor CreateTensorFromRawTensor(ref TFTensor[] tensor_m, TFGraph graph, bool isFromFile = false)
        {

            TFOutput input, output;
            if (isFromFile)
                input = graph.Placeholder(TFDataType.String);
            else
                input = graph.Placeholder(TFDataType.UInt8);
            if (isFromFile)
                output = graph.Cast(graph.DecodePng(contents: input, channels: 3), DstT: TFDataType.Float);
            else
                output = graph.Cast(x: input, DstT: TFDataType.Float);
            output = graph.ExpandDims(input: output, dim: graph.Const(0));

            using (var sess = new TFSession(graph))
            {
                var nor = sess.Run(
                    inputs: new[] { input },
                    inputValues: tensor_m,
                    outputs: new[] { output }
                    );
                return nor[0];
            }
        }




    }
}

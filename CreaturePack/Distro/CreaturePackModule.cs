/******************************************************************************
 * Creature Runtimes License
 * 
 * Copyright (c) 2015, Kestrel Moon Studios
 * All rights reserved.
 * 
 * Preamble: This Agreement governs the relationship between Licensee and Kestrel Moon Studios(Hereinafter: Licensor).
 * This Agreement sets the terms, rights, restrictions and obligations on using [Creature Runtimes] (hereinafter: The Software) created and owned by Licensor,
 * as detailed herein:
 * License Grant: Licensor hereby grants Licensee a Sublicensable, Non-assignable & non-transferable, Commercial, Royalty free,
 * Including the rights to create but not distribute derivative works, Non-exclusive license, all with accordance with the terms set forth and
 * other legal restrictions set forth in 3rd party software used while running Software.
 * Limited: Licensee may use Software for the purpose of:
 * Running Software on Licensee’s Website[s] and Server[s];
 * Allowing 3rd Parties to run Software on Licensee’s Website[s] and Server[s];
 * Publishing Software’s output to Licensee and 3rd Parties;
 * Distribute verbatim copies of Software’s output (including compiled binaries);
 * Modify Software to suit Licensee’s needs and specifications.
 * Binary Restricted: Licensee may sublicense Software as a part of a larger work containing more than Software,
 * distributed solely in Object or Binary form under a personal, non-sublicensable, limited license. Such redistribution shall be limited to unlimited codebases.
 * Non Assignable & Non-Transferable: Licensee may not assign or transfer his rights and duties under this license.
 * Commercial, Royalty Free: Licensee may use Software for any purpose, including paid-services, without any royalties
 * Including the Right to Create Derivative Works: Licensee may create derivative works based on Software, 
 * including amending Software’s source code, modifying it, integrating it into a larger work or removing portions of Software, 
 * as long as no distribution of the derivative works is made
 * 
 * THE RUNTIMES IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE RUNTIMES OR THE USE OR OTHER DEALINGS IN THE
 * RUNTIMES.
 *****************************************************************************/

using UnityEngine;
using System.Collections;
using MiniMessagePack;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CreaturePackModule
{
    public class Pair<T1, T2>
    {
        public T1 first { get; private set; }
        public T2 second { get; private set; }
        internal Pair(T1 firstIn, T2 secondIn)
        {
            first = firstIn;
            second = secondIn;
        }
    }

    public class CreatureConsts
    {
        public static int SPLIT_VTYPE_IDX = 0;
        public static int SPLIT_CLIP_NAME_IDX = 1;
        public static int SPLIT_CLIP_RANGE_IDX = 2;
        public static int SPLIT_CLIP_ANIM_IDX = 4;
    }

    public class CreatureTimeSample
    {
        public int beginTime, endTime, dataIdx;

        public CreatureTimeSample(int beginTimeIn, int endTimeIn, int dataIdxIn)
        {
            init(beginTimeIn, endTimeIn, dataIdxIn);
        }

        public CreatureTimeSample()
        {
            init(0, 0, -1);
        }

        public void init(int beginTimeIn, int endTimeIn, int dataIdxIn)
        {
            beginTime = beginTimeIn;
            endTime = endTimeIn;
            dataIdx = dataIdxIn;
        }

        public int getAnimPointsOffset()
        {
            if (dataIdx < 0)
            {
                return -1; // invalid
            }

            return dataIdx + 1;
        }

        public int getAnimUvsOffset()
        {
            if (dataIdx < 0)
            {
                return -1; // invalid
            }

            return dataIdx + 2;
        }

        public int getAnimColorsOffset()
        {
            if (dataIdx < 0)
            {
                return -1; // invalid
            }

            return dataIdx + 3;
        }

    }

    public struct CreaturePackSampleData
    {
        public int firstSampleIdx, secondSampleIdx;
        public float sampleFraction;

        public CreaturePackSampleData(int firstSampleIdxIn, int secondSampleIdxIn, float sampleFractionIn)
        {
            firstSampleIdx = firstSampleIdxIn;
            secondSampleIdx = secondSampleIdxIn;
            sampleFraction = sampleFractionIn;
        }
    }

    public class CreaturePackAnimClip
    {
        public int startTime, endTime;
        public SortedDictionary<int, CreatureTimeSample> timeSamplesMap = new SortedDictionary<int, CreatureTimeSample>();
        public  int dataIdx;
        public bool firstSet;
        public object[] fileData;

        public CreaturePackAnimClip()
        {
            initData(-1, null);
        }

        public CreaturePackAnimClip(int dataIdxIn, object[] fileDataIn)
        {
            initData(dataIdxIn, fileDataIn);
        }

        public void initData(int dataIdxIn, object[] fileDataIn)
        {
            dataIdx = dataIdxIn;
            startTime = 0;
            endTime = 0;
            firstSet = false;
            timeSamplesMap = new SortedDictionary<int, CreatureTimeSample>();
            fileData = fileDataIn;
        }

        public CreaturePackSampleData sampleTime(float timeIn)
        {

            int lookupTime = (int)(Mathf.Round(timeIn));
            if (timeSamplesMap.ContainsKey(lookupTime) == false)
            {
                float curTime = startTime;
                return new CreaturePackSampleData((int)curTime, (int)curTime, 0.0f);
            }

            float lowTime = (float)timeSamplesMap[lookupTime].beginTime;
            float highTime = (float)timeSamplesMap[lookupTime].endTime;

            if ((highTime - lowTime) <= 0.0001)
            {
                return new CreaturePackSampleData((int)lowTime, (int)highTime, 0.0f);
            }

            float curFraction = (timeIn - lowTime) / (highTime - lowTime);

            return new CreaturePackSampleData((int)lowTime, (int)highTime, curFraction);
        }

        public float correctTime(float timeIn, bool withLoop)
        {
            if (withLoop == false)
            {
                if (timeIn < startTime)
                {
                    return startTime;
                }
                else if (timeIn > endTime)
                {
                    return endTime;
                }
            }
            else
            {
                if (timeIn < startTime)
                {
                    return endTime;
                }
                else if (timeIn > endTime)
                {
                    return startTime;
                }
            }

            return timeIn;
        }

        public void addTimeSample(int timeIn, int dataIdxIn)
        {
            CreatureTimeSample newTimeSample = new CreatureTimeSample(timeIn, timeIn, dataIdxIn);
            timeSamplesMap[timeIn] = newTimeSample;

            if (firstSet == false)
            {
                firstSet = true;
                startTime = timeIn;
                endTime = timeIn;
            }
            else
            {
                if (startTime > timeIn)
                {
                    startTime = timeIn;
                }

                if (endTime < timeIn)
                {
                    endTime = timeIn;
                }
            }
        }

        public void finalTimeSamples()
        {
            int oldTime = startTime;
            List<int> sorted_keys = new List<int>();

            foreach (var curData in timeSamplesMap)
            {
                sorted_keys.Add(curData.Key);
            }

            sorted_keys.Sort();

            foreach (var curTime in sorted_keys)
            {
                if (curTime != oldTime)
                {
                    for (int fillTime = oldTime + 1; fillTime < curTime; fillTime++)
                    {
                        CreatureTimeSample newTimeSample = new CreatureTimeSample(oldTime, curTime, -1);
                        timeSamplesMap[fillTime] = newTimeSample;
                    }

                    oldTime = curTime;
			    }
            }		
	    }
    }

    // This class loads a single Animation from a Split Creature Pack Anim Data objects list
    public class CreaturePackSplitAnimClip : CreaturePackAnimClip
    {
        public CreaturePackSplitAnimClip(object[] dataList)
        {
            var readData = dataList;
            int vType = (int)readData[CreatureConsts.SPLIT_VTYPE_IDX];

            if(vType == 0)
            {
                object[] localRange = (object[])readData[CreatureConsts.SPLIT_CLIP_RANGE_IDX];
                startTime = (int)localRange[0];
                endTime = (int)localRange[1];

                initData(CreatureConsts.SPLIT_CLIP_ANIM_IDX, readData);
  
                for(int k = dataIdx; k < fileData.Length; k += 4)
                {
                    int cur_time = (int)(float)(fileData[k]);
                    addTimeSample(cur_time, (int)k);
                }

                finalTimeSamples();
            }
            else
            {
                // Invalid version type
                System.Diagnostics.Debug.Assert(true);
            }
        }
    }

    // This is the class the loads in Creature Pack Data from the byte stream
    public class CreaturePackLoader
    {
        public class graphNode
        {
            public int idx;
            public List<int> neighbours = new List<int>();
            public bool visited;

            public graphNode()
            {
                init(-1);
            }

            public graphNode(int idxIn)
            {
                init(idxIn);
            }

            public void init(int idxIn)
            {
                idx = idxIn;
                visited = false;
            }
        }

        public uint[] indices;
        public float[] uvs;
        public float[] points;
        public Dictionary<string, CreaturePackAnimClip> animClipMap = new Dictionary<string, CreaturePackAnimClip>();

        public object[] fileData;
        public object[] headerList;
        public object[] animPairsOffsetList;
        public List<Pair<uint, uint>> meshRegionsList;
        public float dMinX, dMinY, dMaxX, dMaxY;

        public CreaturePackLoader()
        {
            // do nothing
        }

        public CreaturePackLoader(Stream byteStream, bool loadMultiCore)
        {
            runDecoder(byteStream, loadMultiCore);
            meshRegionsList = findConnectedRegions();
        }
        
        public void updateIndices(int idx)
        {
            var cur_data = (object[])fileData[idx];
            for (var i = 0; i < cur_data.Length; i++)
            {
                indices[i] = (uint)(int)(cur_data[i]);
            }
        }

        public void updatePoints(int idx)
        {
            var cur_data = (object[])fileData[idx];
            for (var i = 0; i < cur_data.Length; i++)
            {
                points[i] =  (float)(cur_data[i]);
            }
        }

        public void updateUVs(int idx)
        {
            var cur_data = (object[])fileData[idx];
            for (var i = 0; i < cur_data.Length; i++)
            {
                uvs[i] = (float)(cur_data[i]);
            }
        }

        public int getAnimationNum()
        {
            int sum = 0;
            for (int i = 0; i < headerList.Length; i++)
            {
                if ((string)headerList[i] == "animation")
                {
                    sum++;
                }
            }
            
            return sum;
        }

        public string hasDeformCompress()
        {
            for (int i = 0; i < headerList.Length; i++)
            {
                if ((string)headerList[i] == "deform_comp1")
                {
                    return "deform_comp1";
                }
                else if ((string)headerList[i] == "deform_comp2")
                {
                    return "deform_comp2";
                }
                else if ((string)headerList[i] == "deform_comp1_1")
                {
                    return "deform_comp1_1";
                }
            }

            return "";
        }

        public void fillDeformRanges()
        {
            if(hasDeformCompress().Length > 0)
            {
                var cur_ranges_offset = getAnimationOffsets(getAnimationNum());
                var cur_ranges = (object[])fileData[cur_ranges_offset.first];
                dMinX = (float)(cur_ranges[0]);
                dMinY = (float)(cur_ranges[1]);
                dMaxX = (float)(cur_ranges[2]);
                dMaxY = (float)(cur_ranges[3]);
            }
        }

        public Pair<int, int> getAnimationOffsets(int idx)
        {
            return new Pair<int, int>(
               (int)(animPairsOffsetList[idx * 2]), 
               (int)(animPairsOffsetList[idx * 2 + 1]));
        }

        public int getBaseOffset()
        {
            return 0;
        }

        public int getAnimPairsListOffset()
        {
            return 1;
        }

        public int getBaseIndicesOffset()
        {
            return getAnimPairsListOffset() + 1;
        }

        public int getBasePointsOffset()
        {
            return getAnimPairsListOffset() + 2;
        }

        public int getBaseUvsOffset()
        {
            return getAnimPairsListOffset() + 3;
        }

        public int getNumIndices()
        {
            var curData = (object[])fileData[getBaseIndicesOffset()];
            return curData.Length;
        }

        public int getNumPoints()
        {
            var curData = (object[])fileData[getBasePointsOffset()];
            return curData.Length;
        }

        public int getNumUvs() 
        {
            var curData = (object[])fileData[getBaseUvsOffset()];
            return curData.Length;
        }

        public void runDecoder(Stream byteStream, bool loadMultiCore)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var newReader = new MiniMessagePacker();
            fileData = (object[])newReader.Unpack(byteStream);

            stopWatch.Stop();
#if (CREATURE_PACK_DEBUG)
            UnityEngine.Debug.Log("---Data Decode time: " + stopWatch.ElapsedMilliseconds);
#endif
            stopWatch.Reset();

            headerList = (object[])fileData[getBaseOffset()];
            animPairsOffsetList = (object[])fileData[getAnimPairsListOffset()];
            
            // init basic points and topology structure
            indices = new uint[getNumIndices()]; 
            points = new float[getNumPoints()];
            uvs = new float[getNumUvs()];
            
            updateIndices(getBaseIndicesOffset());
            updatePoints(getBasePointsOffset());
            updateUVs(getBaseUvsOffset());

            fillDeformRanges();

            stopWatch.Start();

            if (loadMultiCore)
            {
                finalAllPointSamplesThreaded();
            }
            else
            {
                finalAllPointSamples();
            }

            stopWatch.Stop();
#if (CREATURE_PACK_DEBUG)
            UnityEngine.Debug.Log("---Data Packaging time: " + stopWatch.ElapsedMilliseconds);
#endif

            // init Animation Clip Map		
            for (int i = 0; i < getAnimationNum(); i++)
            {
                var curOffsetPair = getAnimationOffsets(i);
                
                var animName = (string)(fileData[curOffsetPair.first]);
                var k = curOffsetPair.first;
                k++;
                var newClip = new CreaturePackAnimClip(k, fileData);
                    
                while(k < curOffsetPair.second)
                {
                    int cur_time = (int)(float)(fileData[k]);
                    newClip.addTimeSample(cur_time, (int)k);
                        
                    k += 4;
                }
                    
                newClip.finalTimeSamples();
                animClipMap[animName] = newClip;
            }
        }

        class FinalPointsProcessData
        {
            public int idx;
            public object[] fileData;
            private CreaturePackLoader parentLoader;
            public float dMinX, dMinY, dMaxX, dMaxY;
            public float[] points;

            public FinalPointsProcessData(int idxIn, CreaturePackLoader loaderIn)
            {
                idx = idxIn;
                parentLoader = loaderIn;
                dMinX = parentLoader.dMinX;
                dMinY = parentLoader.dMinY;
                dMaxX = parentLoader.dMaxX;
                dMaxY = parentLoader.dMaxY;

                points = parentLoader.points;
                fileData = parentLoader.fileData;
            }

            public virtual string hasDeformCompress()
            {
                return parentLoader.hasDeformCompress();
            }

            public virtual Pair<int, int> getAnimationOffsets(int idx)
            {
                return parentLoader.getAnimationOffsets(idx);
            }
        };

        class FinalPointsProcessSplitData : FinalPointsProcessData
        {
            public FinalPointsProcessSplitData(CreaturePackLoader loaderIn, object[] dataList)
                : base(CreatureConsts.SPLIT_CLIP_ANIM_IDX, loaderIn)
            {
                fileData = dataList;   
            }

            public override Pair<int, int> getAnimationOffsets(int idx)
            {
                int start_val = CreatureConsts.SPLIT_CLIP_ANIM_IDX - 1;
                int end_val = (fileData.Length - 1);
                return new Pair<int, int>(CreatureConsts.SPLIT_CLIP_ANIM_IDX - 1, end_val);
            }
        }

        private static void processPerFinalAllPointsSample(object objIn)
        {
            FinalPointsProcessData curProcessData = (FinalPointsProcessData)objIn;
            int idx = curProcessData.idx;

            var deformCompressType = curProcessData.hasDeformCompress();
            bool has_deform_compress = (deformCompressType.Length > 0);
            var curOffsetPair = curProcessData.getAnimationOffsets(idx);
            var k = curOffsetPair.first;
            k++;
            byte[] bytes2Data = new byte[2];

            while (k < curOffsetPair.second)
            {
                var pts_raw_array = curProcessData.fileData[k + 1];
                int raw_num = 0;
                if (pts_raw_array.GetType() == typeof(object[]))
                {
                    raw_num = ((object[])pts_raw_array).Length;
                }
                else if (pts_raw_array.GetType() == typeof(byte[]))
                {
                    if (deformCompressType == "deform_comp1_1")
                    {
                        raw_num = ((byte[])pts_raw_array).Length / 2;
                    }
                    else 
                    {
                        raw_num = ((byte[])pts_raw_array).Length;
                    }
                }

                var final_pts_array = new float[raw_num];
                if (!has_deform_compress)
                {
                    var pts_array = (object[])pts_raw_array;
                    for (int m = 0; m < raw_num; m++)
                    {
                        final_pts_array[m] = (float)pts_array[m];
                    }
                }
                else
                {
                    object[] pts_obj_array = null;
                    byte[] pts_byte_array = null;
                    float recp_x = 0, recp_y = 9;
                    float numBuckets = 0.0f;
                    if (deformCompressType == "deform_comp1")
                    {
                        pts_obj_array = (object[])pts_raw_array;
                        numBuckets = 65535.0f;
                    }
                    else if (deformCompressType == "deform_comp2")
                    {
                        pts_byte_array = (byte[])pts_raw_array;
                        numBuckets = 255.0f;
                    }
                    else if (deformCompressType == "deform_comp1_1")
                    {
                        pts_byte_array = (byte[])pts_raw_array;
                        numBuckets = 65535.0f;
                    }

                    recp_x = 1.0f / numBuckets * (curProcessData.dMaxX - curProcessData.dMinX);
                    recp_y = 1.0f / numBuckets * (curProcessData.dMaxY - curProcessData.dMinY);
                    int bucketType = 0;
                    if (deformCompressType == "deform_comp1")
                    {
                        bucketType = 1;
                    }
                    else if (deformCompressType == "deform_comp2")
                    {
                        bucketType = 2;
                    }
                    else if (deformCompressType == "deform_comp1_1")
                    {
                        bucketType = 3;
                    }

                    float bucketVal = 0.0f;
                    float setVal = 0.0f;
                    for (int m = 0; m < raw_num; m++)
                    {
                        bucketVal = 0.0f;
                        if (bucketType == 1)
                        {
                            bucketVal = (float)((int)pts_obj_array[m]);
                        }
                        else if (bucketType == 2)
                        {
                            bucketVal = (float)((byte)pts_byte_array[m]);
                        }
                        else if (bucketType == 3)
                        {
                            bytes2Data[0] = pts_byte_array[m * 2];
                            bytes2Data[1] = pts_byte_array[m * 2 + 1];
                            int int_val = (int)BitConverter.ToUInt16(bytes2Data, 0);
                            bucketVal = (float)int_val;
                        }

                        setVal = 0.0f;
                        if (m % 2 == 0)
                        {
                            setVal = bucketVal * recp_x + curProcessData.dMinX;
                            setVal += curProcessData.points[m];
                        }
                        else
                        {
                            setVal = bucketVal * recp_y + curProcessData.dMinY;
                            setVal += curProcessData.points[m];
                        }

                        final_pts_array[m] = setVal;
                    }
                }

                curProcessData.fileData[k + 1] = final_pts_array;

                k += 4;
            }
        }

        public void finalAllPointSamples()
        {
            for (int i = 0; i < getAnimationNum(); i++)
            {
                processPerFinalAllPointsSample(new FinalPointsProcessData(i, this));
            }
        }

        public void finalAllPointSamplesThreaded()
        {
            List<Thread> threadList = new List<Thread>();
            for (int i = 0; i < getAnimationNum(); i++)
            {
                if(threadList.Count < SystemInfo.processorCount)
                {
                    Thread newT = new Thread(new ParameterizedThreadStart(processPerFinalAllPointsSample));
                    threadList.Add(newT);
                    threadList[i].Start(new FinalPointsProcessData(i, this));
                }
                else
                {
                    processPerFinalAllPointsSample(new FinalPointsProcessData(i, this));
                }
            }

            for(int i = 0; i < threadList.Count; i++)
            {
                threadList[i].Join();
            }

#if (CREATURE_PACK_DEBUG)
            UnityEngine.Debug.Log("---Loader Threads spawned: " + threadList.Count);
#endif
        }

        public string GetFirstAnimClipName()
        {
            foreach(var curClip in animClipMap)
            {
                return curClip.Key;
            }

            return "";
        }

        public Dictionary<uint, graphNode> formUndirectedGraph()
        {
            var retGraph = new Dictionary<uint, graphNode>();
            var numTriangles = getNumIndices() / 3;
            for (var i = 0; i < numTriangles; i++)
            {
                uint[] triIndices = new uint[3];
                triIndices[0] = indices[i * 3];
                triIndices[1] = indices[i * 3 + 1];
                triIndices[2] = indices[i * 3 + 2];

                foreach (var triIndex in triIndices)
                {
                    if (retGraph.ContainsKey(triIndex) == false)
                    {
                        retGraph[triIndex] = new graphNode((int)triIndex);
                    }

                    var curGraphNode = retGraph[triIndex];
                    for (var j = 0; j < triIndices.Length; j++)
                    {
                        var cmpIndex = triIndices[j];
                        if (cmpIndex != triIndex)
                        {
                            curGraphNode.neighbours.Add((int)cmpIndex);
                        }
                    }
                }
            }

            return retGraph;
        }

        public List<uint>
        regionsDFS(Dictionary<uint, graphNode> graph, int idx)
        {
            List<uint> retData = new List<uint>();
            if (graph[(uint)idx].visited)
            {
                return retData;
            }

            Stack<uint> gstack = new Stack<uint>();
            gstack.Push((uint)idx);

            while (gstack.Count > 0)
            {
                var curIdx = gstack.Pop();

                var curNode = graph[curIdx];
                if (curNode.visited == false)
                {
                    curNode.visited = true;
                    retData.Add((uint)curNode.idx);
                    // search all connected for curNode
                    foreach (var neighbourIdx in curNode.neighbours)
                    {
                        gstack.Push((uint)neighbourIdx);
                    }
                }
            }

            return retData;
        }

        public List<Pair<uint, uint>>
        findConnectedRegions()
        {
            List<Pair<uint, uint>> regionsList = new List<Pair<uint, uint>>();
            Dictionary<uint, graphNode> graph = formUndirectedGraph();

            // Run depth first search
            uint regionIdx = 0;
            for (var i = 0; i < getNumIndices(); i++)
            {
                var curIdx = indices[i];
                if (graph[curIdx].visited == false)
                {
                    var indicesList = regionsDFS(graph, (int)curIdx);
                    indicesList.Sort();

                    regionsList.Add(
                        new Pair<uint, uint>(indicesList[0], indicesList[indicesList.Count - 1]));

                    regionIdx++;
                }
            }

            return regionsList;
        }

        // Adds in a new animation clip from a CreaturePack Anim Data byte stream
        // This means you are using the separately exported CreaturePack Animation Data files
        public void addSplitAnimClip(Stream byteStream, CreaturePackPlayer playerIn)
        {
            var newReader = new MiniMessagePacker();
            var unpackData = newReader.Unpack(byteStream);
            var readData = (object[])unpackData;

            // Add in new clip data
            var splitClip = new CreaturePackSplitAnimClip(readData);
            var clipName = (string)readData[CreatureConsts.SPLIT_CLIP_NAME_IDX];
            animClipMap[clipName] = splitClip;
            playerIn.runTimeMap[clipName] = splitClip.startTime;

            UnityEngine.Debug.Log("Adding Pack Split Animation: " + clipName);

            // Process clip point data
            var splitProcessObj = new FinalPointsProcessSplitData(this, readData);
            processPerFinalAllPointsSample(splitProcessObj);
        }

        // Removes the designated animation clip from the list of loaded animation clips
        public bool removeSplitAnimClip(string animName, Stream byteStream, CreaturePackPlayer playerIn)
        {
            if(!animClipMap.ContainsKey(animName))
            {
                return false;
            }

            animClipMap.Remove(animName);
            playerIn.runTimeMap.Remove(animName);
            return true;
        }
    }

    // Base Player class that target renderers use
    public class CreaturePackPlayer
    {
        public class CreaturePackMeshRange
        {
            public int start_idx = 0;
            public int end_idx = 0;

            public CreaturePackMeshRange(int start_idx_in, int end_idx_in)
            {
                start_idx = start_idx_in;
                end_idx = end_idx_in;
            }
        }

        public CreaturePackLoader data;
        public float[] render_uvs;
        public byte[] render_colors;
        public float[] render_points;
        int renders_base_size;
        public Dictionary<string, float> runTimeMap;
        public bool isPlaying, isLooping;
        public string activeAnimationName, prevAnimationName;
        float animBlendFactor, animBlendDelta;
        public List<CreaturePackMeshRange> mesh_ranges = new List<CreaturePackMeshRange>();

        public CreaturePackPlayer(CreaturePackLoader dataIn)
        {
            data = dataIn;
            createRuntimeMap();
            isPlaying = true;
            isLooping = true;
            animBlendFactor = 0;
            animBlendDelta = 0;
                    
            // create data buffers
            renders_base_size = data.getNumPoints() / 2;
            mesh_ranges.Add(new CreaturePackMeshRange(0, renders_base_size - 1));

            render_points =new float[getRenderPointsLength()];
            render_uvs = new float[getRenderUVsLength()];
            render_colors = new byte[getRenderColorsLength()];
            
            for (var i = 0; i < (int)getRenderColorsLength(); i++)
            {
                render_colors[i] = 255;
            }
            
            for (var i = 0; i < (int)getRenderUVsLength(); i++)
            {
                render_uvs[i] = data.uvs[i];
            }       
        }

        public int getRenderColorsLength() {
            return (int)renders_base_size * 4; 
        }

        public int getRenderPointsLength() {
            return (int)renders_base_size * 3; 
        }

        public int getRenderUVsLength() {
            return (int)renders_base_size * 2; 
        }
        
        void createRuntimeMap()
        {
            runTimeMap = new Dictionary<string, float>();
            bool firstSet = false;
            foreach(var curData in data.animClipMap)
            {
                var animName = curData.Key;
                
                if (firstSet == false)
                {
                    firstSet = true;
                    activeAnimationName = animName;
                    prevAnimationName = animName;
                }
                
                var animClip = data.animClipMap[animName];
                runTimeMap[animName] = animClip.startTime;
            }
            
        }

        // Sets an active animation without blending
        public bool setActiveAnimation(string nameIn)
        {
            if (runTimeMap.ContainsKey(nameIn))
            {
                activeAnimationName = nameIn;
                prevAnimationName = nameIn;
                runTimeMap[activeAnimationName] = data.animClipMap[activeAnimationName].startTime;
                
                return true;
            }
            
            return false;
        }

        // Smoothly blends to a target animation
        public void blendToAnimation(string nameIn, float blendDelta)
        {
            if(nameIn == activeAnimationName)
            {
                return;
            }

            if (runTimeMap.ContainsKey(nameIn)) {
                prevAnimationName = activeAnimationName;
                activeAnimationName = nameIn;
                animBlendFactor = 0;
                animBlendDelta = blendDelta;

                runTimeMap[activeAnimationName] = data.animClipMap[activeAnimationName].startTime;
            }
        }

        public string resolveAnimName(string nameIn)
        {
            if (nameIn.Length == 0)
            {
                return activeAnimationName;
            }

            return nameIn;
        }

        public void setRunTime(float timeIn, string animName)
        {	
            runTimeMap[resolveAnimName(animName)] = data.animClipMap[resolveAnimName(animName)].correctTime(timeIn, isLooping);
        }

        public float getRunTime(string animName)
        {
            return runTimeMap[resolveAnimName(animName)];
        }

        // Steps the animation by a delta time
        public void stepTime(float deltaTime)
        {
            if (activeAnimationName == prevAnimationName)
            {
                // No Blending
                setRunTime(getRunTime(activeAnimationName) + deltaTime, activeAnimationName);
            }
            else
            {
                // Blending
                setRunTime(getRunTime(activeAnimationName) + deltaTime, activeAnimationName);
                setRunTime(getRunTime(prevAnimationName) + deltaTime, prevAnimationName);
            }

            // update blending
            animBlendFactor += animBlendDelta;
            if (animBlendFactor > 1)
            {
                animBlendFactor = 1;
            }
        }
        
        float interpScalar(float val1, float val2, float fraction)
        {
            return ((1.0f - fraction) * val1) + (fraction * val2);
        }

        // Call this before a render to update the render data
        public void syncRenderData() { 
        {
            // Points blending
            if (activeAnimationName == prevAnimationName)
            {
                var cur_clip = data.animClipMap[activeAnimationName];
                // no blending
                var cur_clip_info = cur_clip.sampleTime(getRunTime(activeAnimationName));
                CreatureTimeSample low_data = cur_clip.timeSamplesMap[cur_clip_info.firstSampleIdx];
                CreatureTimeSample high_data = cur_clip.timeSamplesMap[cur_clip_info.secondSampleIdx];

                float[] anim_low_points = (float[])cur_clip.fileData[low_data.getAnimPointsOffset()];
                float[] anim_high_points = (float[])cur_clip.fileData[high_data.getAnimPointsOffset()];
                
                foreach (var cur_range in mesh_ranges)
                {
                        // This is the parallel version
                        //Parallel.For(cur_range.start_idx, cur_range.end_idx + 1, i =>
                        for (var i = cur_range.start_idx; i <= cur_range.end_idx; i++) // This is the sequential version
                        {
                            for (var j = 0; j < 2; j++)
                            {
                                var low_val = (float)(anim_low_points[i * 2 + j]);
                                var high_val = (float)(anim_high_points[i * 2 + j]);
                                render_points[i * 3 + j] = interpScalar(low_val, high_val, cur_clip_info.sampleFraction);
                            }

                            render_points[i * 3 + 2] = 0.0f;
                        }
                    }
            }
            else {
                // blending
                
                // Active Clip
                var active_clip =  data.animClipMap[activeAnimationName];
                
                var active_clip_info = active_clip.sampleTime(getRunTime(activeAnimationName));
                CreatureTimeSample active_low_data = active_clip.timeSamplesMap[active_clip_info.firstSampleIdx];
                CreatureTimeSample active_high_data = active_clip.timeSamplesMap[active_clip_info.secondSampleIdx];

                float[] active_anim_low_points = (float[])active_clip.fileData[active_low_data.getAnimPointsOffset()];
                float[] active_anim_high_points = (float[])active_clip.fileData[active_high_data.getAnimPointsOffset()];
                
                // Previous Clip
                var prev_clip =  data.animClipMap[prevAnimationName];
                
                var prev_clip_info = prev_clip.sampleTime(getRunTime(prevAnimationName));
                CreatureTimeSample prev_low_data = prev_clip.timeSamplesMap[prev_clip_info.firstSampleIdx];
                CreatureTimeSample prev_high_data = prev_clip.timeSamplesMap[prev_clip_info.secondSampleIdx];

                float[] prev_anim_low_points = (float[])prev_clip.fileData[prev_low_data.getAnimPointsOffset()];
                float[] prev_anim_high_points = (float[])prev_clip.fileData[prev_high_data.getAnimPointsOffset()];

                foreach (var cur_range in mesh_ranges)
                    {
                        // This is the parallel version
                        //Parallel.For(cur_range.start_idx, cur_range.end_idx + 1, i =>
                        for (var i = cur_range.start_idx; i <= cur_range.end_idx; i++) // This is the sequential version
                        {
                            for (var j = 0; j < 2; j++)
                            {
                                var active_low_val = (float)(active_anim_low_points[i * 2 + j]);
                                var active_high_val = (float)(active_anim_high_points[i * 2 + j]);
                                var active_val = interpScalar(active_low_val, active_high_val, active_clip_info.sampleFraction);

                                var prev_low_val = (float)(prev_anim_low_points[i * 2 + j]);
                                var prev_high_val = (float)(prev_anim_high_points[i * 2 + j]);
                                var prev_val = interpScalar(prev_low_val, prev_high_val, prev_clip_info.sampleFraction);

                                render_points[i * 3 + j] = interpScalar(prev_val, active_val, animBlendFactor);
                            }

                            render_points[i * 3 + 2] = 0.0f;
                        }
                    }
                }
            
            // Colors
            {
                var cur_clip =  data.animClipMap[activeAnimationName];
                // no blending
                var cur_clip_info = cur_clip.sampleTime(getRunTime(activeAnimationName));
                CreatureTimeSample low_data = cur_clip.timeSamplesMap[cur_clip_info.firstSampleIdx];
                CreatureTimeSample high_data = cur_clip.timeSamplesMap[cur_clip_info.secondSampleIdx];

                object[] anim_low_colors = (object[])cur_clip.fileData[low_data.getAnimColorsOffset()];
                object[] anim_high_colors = (object[])cur_clip.fileData[high_data.getAnimColorsOffset()];

                if ((anim_low_colors.Length == getRenderColorsLength())
                    && (anim_high_colors.Length == getRenderColorsLength())) {
                    for (var i = 0; i < getRenderColorsLength(); i++) // This is the sequential version
                    // This is the parallel version
                    //Parallel.For(0, getRenderColorsLength(), i =>
                    {
                        float low_val = (float)(int)(anim_low_colors[i]);
                        float high_val = (float)(int)(anim_high_colors[i]);

                        render_colors[i] = (byte)interpScalar(low_val, high_val, cur_clip_info.sampleFraction);

                    }
                }
            }
        
                // UVs
                {
                    var cur_clip =  data.animClipMap[activeAnimationName];
                    var cur_clip_info = cur_clip.sampleTime(getRunTime(activeAnimationName));
                    CreatureTimeSample low_data = cur_clip.timeSamplesMap[cur_clip_info.firstSampleIdx];
                    object[] anim_uvs = (object[])cur_clip.fileData[low_data.getAnimUvsOffset()];

                    if (anim_uvs.Length == getRenderUVsLength())
                    {
                        for (var i = 0; i < getRenderUVsLength(); i++) // This is the sequential version
                        // This is the parallel version
                        //Parallel.For(0, getRenderUVsLength(), i =>
                        {
                            render_uvs[i] = (float)(anim_uvs[i]);
                        }
                    }
                }		
            }
        }
        
    } 
}

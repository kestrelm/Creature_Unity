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

    public class CreaturePackSampleData
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

        public CreaturePackAnimClip()
        {
            initData(-1);
        }

        public CreaturePackAnimClip(int dataIdxIn)
        {
            initData(dataIdxIn);
        }

        public void initData(int dataIdxIn)
        {
            dataIdx = dataIdxIn;
            startTime = 0;
            endTime = 0;
            firstSet = false;
            timeSamplesMap = new SortedDictionary<int, CreatureTimeSample>();
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

        public CreaturePackLoader(Stream byteStream)
        {
            runDecoder(byteStream);
            meshRegionsList = findConnectedRegions();
        }
        
        public void updateIndices(int idx)
        {
            var cur_data = (object[])fileData[idx];
            for (var i = 0; i < cur_data.Length; i++)
            {
                indices[i] = (uint)(long)(cur_data[i]);
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
               (int)(long)(animPairsOffsetList[idx * 2]), 
               (int)(long)(animPairsOffsetList[idx * 2 + 1]));
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

        public void runDecoder(Stream byteStream)
        {
            var newReader = new MiniMessagePacker();
            fileData = (object[])newReader.Unpack(byteStream);
            
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
            finalAllPointSamples();

            // init Animation Clip Map		
            for (int i = 0; i < getAnimationNum(); i++)
            {
                var curOffsetPair = getAnimationOffsets(i);
                
                var animName = (string)(fileData[curOffsetPair.first]);
                var k = curOffsetPair.first;
                k++;
                var newClip = new CreaturePackAnimClip(k);
                    
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

        public void finalAllPointSamples()
        {
            var deformCompressType = hasDeformCompress();
            if (deformCompressType.Length == 0)
            {
                return;
            }

            for (int i = 0; i < getAnimationNum(); i++)
            {
                var curOffsetPair = getAnimationOffsets(i);

                var animName = (string)(fileData[curOffsetPair.first]);
                var k = curOffsetPair.first;
                k++;

                while (k < curOffsetPair.second)
                {
                    var pts_raw_array = fileData[k + 1];
                    int raw_num = 0;
                    if(pts_raw_array.GetType() == typeof(object[]))
                    {
                        raw_num = ((object[])pts_raw_array).Length;
                    }
                    else if(pts_raw_array.GetType() == typeof(byte[]))
                    {
                        raw_num = ((byte[])pts_raw_array).Length;
                    }

                    var final_pts_array = new object[raw_num];
                    for (int m = 0; m < raw_num; m++)
                    {
                        float bucketVal = 0.0f;
                        float numBuckets = 0.0f;
                        if(deformCompressType == "deform_comp1")
                        {
                            var pts_array = (object[])pts_raw_array;
                            bucketVal = Convert.ToSingle((long)pts_array[m]);
                            numBuckets = 65535.0f;
                        }
                        else if (deformCompressType == "deform_comp2")
                        {
                            var pts_array = (byte[])pts_raw_array;
                            bucketVal = Convert.ToSingle((byte)pts_array[m]);
                            numBuckets = 255.0f;
                        }

                        float setVal = 0.0f;
                        if(m % 2 == 0)
                        {
                            setVal = (bucketVal / numBuckets * (dMaxX - dMinX)) + dMinX;
                            setVal += points[m];
                        }
                        else
                        {
                            setVal = (bucketVal / numBuckets * (dMaxY - dMinY)) + dMinY;
                            setVal += points[m];
                        }

                        final_pts_array[m] = setVal;
                    }
                    fileData[k + 1] = final_pts_array;

                    k += 4;
                }
            }
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

    }

    // Base Player class that target renderers use
    public class CreaturePackPlayer
    {
        public CreaturePackLoader data;
        public float[] render_uvs;
        public byte[] render_colors;
        public float[] render_points;
        int renders_base_size;
        Dictionary<string, float> runTimeMap;
        public bool isPlaying, isLooping;
        string activeAnimationName, prevAnimationName;
        float animBlendFactor, animBlendDelta;

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

        public void setRunTime(float timeIn)
        {	
            runTimeMap[activeAnimationName] = data.animClipMap[activeAnimationName].correctTime(timeIn, isLooping);
        }

        public float getRunTime()
        {
            return runTimeMap[activeAnimationName];
        }

        // Steps the animation by a delta time
        public void stepTime(float deltaTime)
        {
            setRunTime(getRunTime() + deltaTime);
            
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
                var cur_clip_info = cur_clip.sampleTime(getRunTime());
                CreatureTimeSample low_data = cur_clip.timeSamplesMap[cur_clip_info.firstSampleIdx];
                CreatureTimeSample high_data = cur_clip.timeSamplesMap[cur_clip_info.secondSampleIdx];

                object[] anim_low_points = (object[])data.fileData[low_data.getAnimPointsOffset()];
                object[] anim_high_points = (object[])data.fileData[high_data.getAnimPointsOffset()];
                
                for (var i = 0; i < renders_base_size; i++)
                {
                    for(var j = 0; j < 2; j++)
                    {
                        var low_val = (float)(anim_low_points[i * 2 + j]);
                        var high_val = (float)(anim_high_points[i * 2 + j]);
                        render_points[i * 3 + j] = interpScalar(low_val, high_val, cur_clip_info.sampleFraction);                    
                    }
                    
                    render_points[i * 3 + 2] = 0.0f;
                }
            }
            else {
                // blending
                
                // Active Clip
                var active_clip =  data.animClipMap[activeAnimationName];
                
                var active_clip_info = active_clip.sampleTime(getRunTime());
                CreatureTimeSample active_low_data = active_clip.timeSamplesMap[active_clip_info.firstSampleIdx];
                CreatureTimeSample active_high_data = active_clip.timeSamplesMap[active_clip_info.secondSampleIdx];

                object[] active_anim_low_points = (object[])data.fileData[active_low_data.getAnimPointsOffset()];
                object[] active_anim_high_points = (object[])data.fileData[active_high_data.getAnimPointsOffset()];
                
                // Previous Clip
                var prev_clip =  data.animClipMap[prevAnimationName];
                
                var prev_clip_info = prev_clip.sampleTime(getRunTime());
                CreatureTimeSample prev_low_data = prev_clip.timeSamplesMap[prev_clip_info.firstSampleIdx];
                CreatureTimeSample prev_high_data = prev_clip.timeSamplesMap[prev_clip_info.secondSampleIdx];

                object[] prev_anim_low_points = (object[])data.fileData[prev_low_data.getAnimPointsOffset()];
                object[] prev_anim_high_points = (object[])data.fileData[prev_high_data.getAnimPointsOffset()];

                for (var i = 0; i < renders_base_size; i++)
                {
                    for(var j = 0; j < 2; j++)
                    {
                        var active_low_val = (float)(active_anim_low_points[i * 2 + j]);
                        var active_high_val = (float)(active_anim_high_points[i * 2 + j]);
                        var active_val =  interpScalar(active_low_val, active_high_val, active_clip_info.sampleFraction);

                        var prev_low_val = (float)(prev_anim_low_points[i * 2 + j]);
                        var prev_high_val = (float)(prev_anim_high_points[i * 2 + j]);
                        var prev_val =  interpScalar(prev_low_val, prev_high_val, prev_clip_info.sampleFraction);

                        render_points[i * 3 + j] = interpScalar(prev_val, active_val, animBlendFactor);
                    }
                    
                    render_points[i * 3 + 2] = 0.0f;
                }
            }
            
            // Colors
            {
                var cur_clip =  data.animClipMap[activeAnimationName];
                // no blending
                var cur_clip_info = cur_clip.sampleTime(getRunTime());
                CreatureTimeSample low_data = cur_clip.timeSamplesMap[cur_clip_info.firstSampleIdx];
                CreatureTimeSample high_data = cur_clip.timeSamplesMap[cur_clip_info.secondSampleIdx];

                object[] anim_low_colors = (object[])data.fileData[low_data.getAnimColorsOffset()];
                object[] anim_high_colors = (object[])data.fileData[high_data.getAnimColorsOffset()];

                if ((anim_low_colors.Length == getRenderColorsLength())
                    && (anim_high_colors.Length == getRenderColorsLength())) {
                    for (var i = 0; i < getRenderColorsLength(); i++)
                    {
                        float low_val = (float)(long)(anim_low_colors[i]);
                        float high_val = (float)(long)(anim_high_colors[i]);

                        render_colors[i] = (byte)interpScalar(low_val, high_val, cur_clip_info.sampleFraction);

                    }
                }
            }
        
                // UVs
                {
                    var cur_clip =  data.animClipMap[activeAnimationName];
                    var cur_clip_info = cur_clip.sampleTime(getRunTime());
                    CreatureTimeSample low_data = cur_clip.timeSamplesMap[cur_clip_info.firstSampleIdx];
                    object[] anim_uvs = (object[])data.fileData[low_data.getAnimUvsOffset()];

                    if (anim_uvs.Length == getRenderUVsLength())
                    {
                        for (var i = 0; i < getRenderUVsLength(); i++)
                        {
                            render_uvs[i] = (float)(anim_uvs[i]);
                        }
                    }
                }		
            }
        }
        
    } 
}

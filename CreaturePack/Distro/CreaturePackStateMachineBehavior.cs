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

public class CreaturePackStateMachineBehavior : StateMachineBehaviour
{
    public CreaturePackRenderer pack_renderer;
    public string play_animation_name;
    public bool do_blending = false;
    public float blend_delta = 0.1f;
    public bool custom_clip_range = false;
    public int custom_start_frame = 0;
    public int custom_end_frame = 100;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var creature_renderer = pack_renderer;
        bool process_composite_clip = false;
        creature_renderer.use_composite_clips = false;

        if (creature_renderer.pack_asset.composite_player != null)
        {
            var composite_player = creature_renderer.pack_asset.composite_player;
            if(composite_player.hasCompositeName(play_animation_name))
            {
                process_composite_clip = true;
                creature_renderer.use_composite_clips = true;
                creature_renderer.setCompositeActiveClip(play_animation_name);
            }
        }

        if(!process_composite_clip)
        {
            if (!do_blending)
            {
                creature_renderer.pack_player.setActiveAnimation(play_animation_name);
            }
            else
            {
                creature_renderer.pack_player.blendToAnimation(play_animation_name, blend_delta);
            }

            if (custom_clip_range)
            {
                var pack_data = creature_renderer.pack_player.data;
                if (pack_data.animClipMap.ContainsKey(play_animation_name))
                {
                    creature_renderer.pack_player.isLooping = false;
                    creature_renderer.pack_player.setRunTime(custom_start_frame, "");
                    animator.SetBool("CustomRangeDone", false);
                }
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(custom_clip_range && (custom_end_frame > custom_start_frame))
        {
            var curTransition = animator.GetAnimatorTransitionInfo(layerIndex);
            var pack_player = pack_renderer.pack_player;
            var cur_frame = pack_player.getRunTime("");
            if(cur_frame >= custom_end_frame)
            {
                // Please set a boolean in your animator transition condition called
                // "CustomRangeDone" that will be the trigger for transitioning to a new
                // animation clip
                animator.SetBool("CustomRangeDone", true);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}

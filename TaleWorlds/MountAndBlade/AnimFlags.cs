using System;
using TaleWorlds.DotNet;

namespace TaleWorlds.MountAndBlade;

[EngineStruct("Anim_flags", false)]
[Flags]
public enum AnimFlags : ulong
{
	amf_priority_continue = 1uL,
	amf_priority_jump = 2uL,
	amf_priority_ride = 2uL,
	amf_priority_crouch = 2uL,
	amf_priority_attack = 0xAuL,
	amf_priority_cancel = 0xCuL,
	amf_priority_defend = 0xEuL,
	amf_priority_defend_parry = 0xFuL,
	amf_priority_throw = 0xFuL,
	amf_priority_blocked = 0xFuL,
	amf_priority_parried = 0xFuL,
	amf_priority_kick = 0x21uL,
	amf_priority_jump_end = 0x21uL,
	amf_priority_reload = 0x3CuL,
	amf_priority_mount = 0x40uL,
	amf_priority_equip = 0x46uL,
	amf_priority_rear = 0x4AuL,
	amf_priority_upperbody_while_kick = 0x4BuL,
	amf_priority_striked = 0x50uL,
	amf_priority_fall_from_horse = 0x51uL,
	amf_priority_die = 0x5FuL,
	amf_priority_mask = 0xFFuL,
	anf_disable_agent_agent_collisions = 0x100uL,
	anf_ignore_all_collisions = 0x200uL,
	anf_ignore_static_body_collisions = 0x400uL,
	anf_use_last_step_point_as_data = 0x800uL,
	anf_make_bodyfall_sound = 0x1000uL,
	anf_client_prediction = 0x2000uL,
	anf_keep = 0x4000uL,
	anf_restart = 0x8000uL,
	anf_client_owner_prediction = 0x10000uL,
	anf_make_walk_sound = 0x20000uL,
	anf_disable_hand_ik = 0x40000uL,
	anf_stick_item_to_left_hand = 0x80000uL,
	anf_blends_according_to_look_slope = 0x100000uL,
	anf_synch_with_horse = 0x200000uL,
	anf_use_left_hand_during_attack = 0x400000uL,
	anf_lock_camera = 0x800000uL,
	anf_lock_movement = 0x1000000uL,
	anf_synch_with_movement = 0x2000000uL,
	anf_enable_hand_spring_ik = 0x4000000uL,
	anf_enable_hand_blend_ik = 0x8000000uL,
	anf_synch_with_ladder_movement = 0x10000000uL,
	anf_do_not_keep_track_of_sound = 0x20000000uL,
	anf_reset_camera_height = 0x40000000uL,
	anf_disable_alternative_randomization = 0x80000000uL,
	anf_disable_auto_increment_progress = 0x100000000uL,
	anf_switch_item_between_hands = 0x200000000uL,
	anf_enforce_lowerbody = 0x1000000000uL,
	anf_enforce_all = 0x2000000000uL,
	anf_cyclic = 0x4000000000uL,
	anf_enforce_root_rotation = 0x8000000000uL,
	anf_allow_head_movement = 0x10000000000uL,
	anf_disable_foot_ik = 0x20000000000uL,
	anf_affected_by_movement = 0x40000000000uL,
	anf_update_bounding_volume = 0x80000000000uL,
	anf_align_with_ground = 0x100000000000uL,
	anf_ignore_slope = 0x200000000000uL,
	anf_displace_position = 0x400000000000uL,
	anf_enable_left_hand_ik = 0x800000000000uL,
	anf_ignore_scale_on_root_position = 0x1000000000000uL,
	anf_blend_main_item_bone_entitially = 0x2000000000000uL,
	anf_animation_layer_flags_mask = 0xFFFF000000000uL,
	anf_animation_layer_flags_bits = 0x24uL,
	anf_randomization_weight_1 = 0x1000000000000000uL,
	anf_randomization_weight_2 = 0x2000000000000000uL,
	anf_randomization_weight_4 = 0x4000000000000000uL,
	anf_randomization_weight_8 = 9223372036854775808uL,
	anf_randomization_weight_mask = 17293822569102704640uL
}
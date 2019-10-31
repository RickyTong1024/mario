using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class mario_db : mario_block {

	public List<Sprite> m_sprites;
	
	public override void reset ()
	{
		if (!m_edit_mode)
		{
			change ();
		}
	}

	public override void change()
	{
		List< List<s_t_mission_sub> > arr = game_data._instance.m_arrays[m_world];
		if (m_init_pos.x == 0)
		{
			if (m_init_pos.y == game_data._instance.m_map_data.maps[m_world].y_num - 1)
			{
				if (arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
					set_fx(mario_fx.mf_left);
				}
				else
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
					set_fx(mario_fx.mf_right);
				}
			}
			else
			{
				if (arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
				{
					if (arr[m_init_pos.y + 1][m_init_pos.x].type == 1)
					{
						if (arr[m_init_pos.y + 1][m_init_pos.x + 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[8];
							set_fx(mario_fx.mf_left);
						}
						else
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[7];
							set_fx(mario_fx.mf_left);
						}
					}
					else
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
						set_fx(mario_fx.mf_left);
					}
				}
				else
				{
					if (arr[m_init_pos.y + 1][m_init_pos.x].type == 1)
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[2];
						set_fx(mario_fx.mf_right);
					}
					else
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
						set_fx(mario_fx.mf_right);
					}
				}
			}
		}
		else if (m_init_pos.x == game_data._instance.m_map_data.maps[m_world].x_num - 1)
		{
			if (m_init_pos.y == game_data._instance.m_map_data.maps[m_world].y_num - 1)
			{
				if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1)
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
					set_fx(mario_fx.mf_right);
				}
				else
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
					set_fx(mario_fx.mf_right);
				}
			}
			else
			{
				if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1)
				{
					if (arr[m_init_pos.y + 1][m_init_pos.x].type == 1)
					{
						if (arr[m_init_pos.y + 1][m_init_pos.x - 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[8];
							set_fx(mario_fx.mf_right);
						}
						else
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[7];
							set_fx(mario_fx.mf_right);
						}
					}
					else
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
						set_fx(mario_fx.mf_right);
					}
				}
				else
				{
					if (arr[m_init_pos.y + 1][m_init_pos.x].type == 1)
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[2];
						set_fx(mario_fx.mf_right);
					}
					else
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
						set_fx(mario_fx.mf_right);
					}
				}
			}
		}
		else
		{
			if (m_init_pos.y == game_data._instance.m_map_data.maps[m_world].y_num - 1)
			{
				if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1 && arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[0];
					set_fx(mario_fx.mf_right);
				}
				else if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1)
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
					set_fx(mario_fx.mf_right);
				}
				else if (arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
					set_fx(mario_fx.mf_left);
				}
				else
				{
					this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
					set_fx(mario_fx.mf_right);
				}
			}
			else
			{
				if (arr[m_init_pos.y + 1][m_init_pos.x].type == 1)
				{
					if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1 && arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
					{
						if (arr[m_init_pos.y + 1][m_init_pos.x - 1].type == 1 && arr[m_init_pos.y + 1][m_init_pos.x + 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[4];
							set_fx(mario_fx.mf_right);
						}
						else if (arr[m_init_pos.y + 1][m_init_pos.x - 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[6];
							set_fx(mario_fx.mf_left);
						}
						else if (arr[m_init_pos.y + 1][m_init_pos.x + 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[6];
							set_fx(mario_fx.mf_right);
						}
						else
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[5];
							set_fx(mario_fx.mf_right);
						}
					}
					else if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1)
					{
						if (arr[m_init_pos.y + 1][m_init_pos.x - 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[8];
							set_fx(mario_fx.mf_right);
						}
						else
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[7];
							set_fx(mario_fx.mf_right);
						}
					}
					else if (arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
					{
						if (arr[m_init_pos.y + 1][m_init_pos.x + 1].type == 1)
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[8];
							set_fx(mario_fx.mf_left);
						}
						else
						{
							this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[7];
							set_fx(mario_fx.mf_left);
						}
					}
					else
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[2];
						set_fx(mario_fx.mf_right);
					}
				}
				else
				{
					if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1 && arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[0];
						set_fx(mario_fx.mf_right);
					}
					else if (arr[m_init_pos.y][m_init_pos.x - 1].type == 1)
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
						set_fx(mario_fx.mf_right);
					}
					else if (arr[m_init_pos.y][m_init_pos.x + 1].type == 1)
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[1];
						set_fx(mario_fx.mf_left);
					}
					else
					{
						this.transform.FindChild("sprite").GetComponent<SpriteRenderer>().sprite = m_sprites[3];
						set_fx(mario_fx.mf_right);
					}
				}
			}
		}
	}
}

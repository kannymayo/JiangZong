#!/usr/bin/env python

# Copyright (c) 2013, Rethink Robotics
# All rights reserved.
#
# Redistribution and use in source and binary forms, with or without
# modification, are permitted provided that the following conditions are met:
#
# 1. Redistributions of source code must retain the above copyright notice,
#    this list of conditions and the following disclaimer.
# 2. Redistributions in binary form must reproduce the above copyright
#    notice, this list of conditions and the following disclaimer in the
#    documentation and/or other materials provided with the distribution.
# 3. Neither the name of the Rethink Robotics nor the names of its
#    contributors may be used to endorse or promote products derived from
#    this software without specific prior written permission.
#
# THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
# AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
# IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
# ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
# LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
# CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
# SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
# INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
# CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
# ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
# POSSIBILITY OF SUCH DAMAGE.
"""
Baxter RSDK Joint Torque Example: joint springs
"""

import read_point_right
import argparse

import rospy

from dynamic_reconfigure.server import (
    Server,
)
from std_msgs.msg import (
    Empty,
)

import baxter_interface

from baxter_examples.cfg import (
    JointSpringsExampleConfig,
)


class JointSprings(object):
    """
    Virtual Joint Springs class for torque example.

    @param limb: limb on which to run joint springs example
    @param reconfig_server: dynamic reconfigure server

    JointSprings class contains methods for the joint torque example allowing
    moving the limb to a neutral location, entering torque mode, and attaching
    virtual springs.
    """
    def __init__(self, limb, reconfig_server):
        self._dyn = reconfig_server
        
        #print self.rcmd[5]
        # control parameters
        self._rate = 1000.0  # Hz
        self._missed_cmds = 20.0  # Missed cycles before triggering timeout

        # create our limb instance
        self._limb = baxter_interface.Limb(limb)
        
        self._left_arm = baxter_interface.limb.Limb("left")
        self._right_arm = baxter_interface.limb.Limb("right")
        # initialize parameters
        self.j1=1
        self.i=1
        self.flag=1
        self._springs = dict()
        self._damping = dict()
        self._start_angles = dict()
        self._rcmd=dict()
        #self._rcmd=read_point.read_point('tests', loops=1)
        
        #high stiffness
        #'''
        self._springs['right_s0']=400.0
        self._damping['right_s0']=0.01*pow(self._springs['right_s0'],0.5)
        self._springs['right_s1']=400
        self._damping['right_s1']=0.1*pow(self._springs['right_s1'],0.5)
        self._springs['right_e0']=300
        self._damping['right_e0']=0.2*pow(self._springs['right_e0'],0.5)
        self._springs['right_e1']=300
        self._damping['right_e1']=0.2*pow(self._springs['right_e1'],0.5)
        self._springs['right_w0']=50
        self._damping['right_w0']=0.2*pow(self._springs['right_w0'],0.5)
        self._springs['right_w1']=50
        self._damping['right_w1']=0.2*pow(self._springs['right_w1'],0.5)
        self._springs['right_w2']=50
        self._damping['right_w2']=0.2*pow(self._springs['right_w2'],0.5)
        #'''
        # low stiffness
        '''
        self._springs['right_s0']=60.0
        self._damping['right_s0']=0.01*pow(self._springs['right_s0'],0.5)
        self._springs['right_s1']=60
        self._damping['right_s1']=0.1*pow(self._springs['right_s1'],0.5)
        self._springs['right_e0']=60
        self._damping['right_e0']=0.2*pow(self._springs['right_e0'],0.5)
        self._springs['right_e1']=60
        self._damping['right_e1']=0.2*pow(self._springs['right_e1'],0.5)
        self._springs['right_w0']=30
        self._damping['right_w0']=0.2*pow(self._springs['right_w0'],0.5)
        self._springs['right_w1']=20
        self._damping['right_w1']=0.2*pow(self._springs['right_w1'],0.5)
        self._springs['right_w2']=20
        self._damping['right_w2']=0.2*pow(self._springs['right_w2'],0.5)
        '''
        # EMG stiffness
        #'''
        
        #'''
	# create cuff disable publisher
        cuff_ns = 'robot/limb/' + limb + '/suppress_cuff_interaction'
        self._pub_cuff_disable = rospy.Publisher(cuff_ns, Empty)

        # verify robot is enabled
        print("Getting robot state... ")
        self._rs = baxter_interface.RobotEnable()
        self._init_state = self._rs.state().enabled
        print("Enabling robot... ")
        self._rs.enable()
        print("Running. Ctrl-c to quit")
        #import reference position
        self._rcmd=dict()
        self._rcmd=read_point_right.read_point('writing_cyber', loops=1)
        #self._rcmdl=read_point.read_point('myo_elbowhold', loops=1)
        # create our limb instance
        self._limb = baxter_interface.Limb(limb)
        #print self._rcmd[1]
        self._limb.move_to_joint_positions(self._rcmd[1], 15)
        #self._left_arm.move_to_joint_positions(self._rcmd[1], 15)
        #read reference stiffness from file 
        with open('stiffq_cyber.txt') as f:
             data = [line.split(",") for line in f.readlines()]
             self.out = [(float(k1), float(v1), float(k2), float(v2),float(k3), float(v3),float(k4)) for k1,v1,k2,v2,k3,v3,k4 in data]
             #print len(self.out) 
        #save force and pos of endpoint
        self.ref_stiff = open("/home/baxter/forces_ref0303hrh.dat", "a")
        self.ref_pos = open("/home/baxter/pos_ref0303hrh.dat", "a")
        self.ref_force = open("/home/baxter/command_ref0303hrh.dat", "a")
    def _update_parameters(self):
        for joint in self._limb.joint_names():
            self._springs[joint] = self._dyn.config[joint[-2:] +
                                                    '_spring_stiffness']
            self._damping[joint] = self._dyn.config[joint[-2:] +
                                                    '_damping_coefficient']
    
    def _update_forces(self):
        """
        Calculates the current angular difference between the start position
        and the current joint positions applying the joint torque spring forces
        as defined on the dynamic reconfigure server.
        """
        # get latest spring constants
        #self._start_angles[joint]=read_point1.map_file('tests', self.i+=1)
        #self._update_parameters()
        #'''
        self._springs['right_s0']=400.0
        self._damping['right_s0']=0.01*pow(self._springs['right_s0'],0.5)
        self._springs['right_s1']=400
        self._damping['right_s1']=0.1*pow(self._springs['right_s1'],0.5)
        self._springs['right_e0']=300
        self._damping['right_e0']=0.2*pow(self._springs['right_e0'],0.5)
        self._springs['right_e1']=300
        self._damping['right_e1']=0.2*pow(self._springs['right_e1'],0.5)
        self._springs['right_w0']=50
        self._damping['right_w0']=0.2*pow(self._springs['right_w0'],0.5)
        self._springs['right_w1']=50
        self._damping['right_w1']=0.2*pow(self._springs['right_w1'],0.5)
        self._springs['right_w2']=50
        self._damping['right_w2']=0.2*pow(self._springs['right_w2'],0.5)
        #'''
        '''
        self._springs['right_s0']=60.0
        self._damping['right_s0']=0.02*pow(self._springs['right_s0'],0.5)
        self._springs['right_s1']=60
        self._damping['right_s1']=0.1*pow(self._springs['right_s1'],0.5)
        self._springs['right_e0']=60
        self._damping['right_e0']=0.2*pow(self._springs['right_e0'],0.5)
        self._springs['right_e1']=60
        self._damping['right_e1']=0.2*pow(self._springs['right_e1'],0.5)
        self._springs['right_w0']=30
        self._damping['right_w0']=0.2*pow(self._springs['right_w0'],0.5)
        self._springs['right_w1']=30
        self._damping['right_w1']=0.2*pow(self._springs['right_w1'],0.5)
        self._springs['right_w2']=30
        self._damping['right_w2']=0.2*pow(self._springs['right_w2'],0.5)
        '''
        '''
        x=self.out[self.i]
        self._springs['right_s0']=(11000-x[0])/10882*350+110
        self._damping['right_s0']=0.01*pow(self._springs['right_s0'],0.5)
        self._springs['right_s1']=(3276-x[1])/2219*350+110
        self._damping['right_s1']=0.1*pow(self._springs['right_s1'],0.5)
        self._springs['right_e0']=(6929-x[2])/6012*250+110
        self._damping['right_e0']=0.2*pow(self._springs['right_e0'],0.5)
        self._springs['right_e1']=(3625.6-x[3])/3178*250+110
        self._damping['right_e1']=0.2*pow(self._springs['right_e1'],0.5)
        self._springs['right_w0']=(1362-x[4])/1231*30+60
        self._damping['right_w0']=0.2*pow(self._springs['right_w0'],0.5)
        self._springs['right_w1']=(251-x[5])/197*30+50
        self._damping['right_w1']=0.2*pow(self._springs['right_w1'],0.5)
        self._springs['right_w2']=30
        self._damping['right_w2']=0.2*pow(self._springs['right_w2'],0.5)
        '''
        
        '''
        x=self.out[self.i]
        self._springs['right_s0']=(11000-x[0])/10882*340+60
        self._damping['right_s0']=0.01*pow(self._springs['right_s0'],0.5)
        self._springs['right_s1']=(3276-x[1])/2219*340+60
        self._damping['right_s1']=0.1*pow(self._springs['right_s1'],0.5)
        self._springs['right_e0']=(6929-x[2])/6012*240+60
        self._damping['right_e0']=0.2*pow(self._springs['right_e0'],0.5)
        self._springs['right_e1']=(3625.6-x[3])/3178*240+60
        self._damping['right_e1']=0.2*pow(self._springs['right_e1'],0.5)
        self._springs['right_w0']=(1362-x[4])/1231*20+30
        self._damping['right_w0']=0.2*pow(self._springs['right_w0'],0.5)
        self._springs['right_w1']=(251-x[5])/197*20+20
        self._damping['right_w1']=0.2*pow(self._springs['right_w1'],0.5)
        self._springs['right_w2']=30
        self._damping['right_w2']=0.2*pow(self._springs['right_w2'],0.5)
        #self.j1+=1
        #print self.j1
        '''    
        
        
        # disable cuff interaction
        self._pub_cuff_disable.publish()

        # create our command dict
        cmd = dict()
        # record current angles/velocities
        cur_pos = self._limb.joint_angles()

        cur_vel = self._limb.joint_velocities()
        cur_end_pos=self._limb.endpoint_pose()
        cur_end_force=self._limb.endpoint_effort()
        if self.i==11372:
          self.ref_stiff.close()
          self.ref_pos.close()
        if self.i<11372:
            self.ref_stiff.write(str(cur_end_force)+"\n")
            self.ref_pos.write(str(cur_end_pos)+"\n")
        # calculate current forces
        #print 
        #self._start_angles=self._rcmd[self.i]
        #self._start_angles['right_s0']+=0.001
        #print self.rcmd[self.i]
        #if self.i<10000:
          # self.i+=1
          # self._start_angles['right_s0']+=0.0001
        #else:
          #   print self.i
        #self.i+=1
        if self.i<=len(self._rcmd)-2 :
           self.flag+=1
           if self.flag>=(self._rate/100.0):
              self.flag=0
              self.i+=1
              print self.i
              if self.i==len(self._rcmd)-2:
                 print 'exit '
                 
                 
        for joint in self._start_angles.keys():
            # import start angles
            if len(self._rcmd)-self.i>1:
               self._start_angles[joint] = self._rcmd[self.i][joint]+self.flag*(self._rcmd[self.i+1][joint]-self._rcmd[self.i][joint])/(self._rate/100.0)
               #self._start_angles[joint] = self._rcmd[self.i][joint]
            # spring portion
 
            cmd[joint] = self._springs[joint] * (self._start_angles[joint] -
                                                   cur_pos[joint])
            # damping portion
            #cmd[joint]=0
            cmd[joint] -= self._damping[joint] * cur_vel[joint]
            self.ref_force.write(str(cmd)+"\n")
            
          # tracking current angles  
       # self._start_angles = cur_pos   
            #cmd[joint]=0
        # command new joint torques
        self._limb.set_joint_torques(cmd)
        #print self._start_angles[joint] - cur_pos[joint]
        #print cur_vel[joint]
    
        
    def move_to_neutral(self):
        timeout=15.0
        for joint in self._start_angles.keys():
            self._start_angles[joint] = self._rcmd[0][joint]
        self._limb.move_to_joint_positions(self._start_angles, timeout)
        """
        Moves the limb to neutral location.
        """
        #self._limb.move_to_neutral()

    def attach_springs(self):
        """
        Switches to joint torque mode and attached joint springs to current
        joint positions.
        """
        # record initial joint angles
        self._start_angles = self._limb.joint_angles()
        
        # set control rate
        control_rate = rospy.Rate(self._rate)

        # for safety purposes, set the control rate command timeout.
        # if the specified number of command cycles are missed, the robot
        # will timeout and disable
        self._limb.set_command_timeout((1.0 / self._rate) * self._missed_cmds)

        # loop at specified rate commanding new joint torques
        while not rospy.is_shutdown():
            if not self._rs.state().enabled:
                rospy.logerr("Joint torque example failed to meet "
                             "specified control rate timeout.")
                break
            self._update_forces()
            control_rate.sleep()

    def clean_shutdown(self):
        """
        Switches out of joint torque mode to exit cleanly
        """
        print("\nExiting example...")
        self._limb.exit_control_mode()
        if not self._init_state and self._rs.state().enabled:
            print("Disabling robot...")
            self._rs.disable()


def main():
    """RSDK Joint Torque Example: Joint Springs

    Moves the specified limb to a neutral location and enters
    torque control mode, attaching virtual springs (Hooke's Law)
    to each joint maintaining the start position.

    Run this example on the specified limb and interact by
    grabbing, pushing, and rotating each joint to feel the torques
    applied that represent the virtual springs attached.
    You can adjust the spring constant and damping coefficient
    for each joint using dynamic_reconfigure.
    """
    arg_fmt = argparse.RawDescriptionHelpFormatter
    parser = argparse.ArgumentParser(formatter_class=arg_fmt,
                                     description=main.__doc__)
    parser.add_argument(
        '-l', '--limb', dest='limb', required=True, choices=['left', 'right'],
        help='limb on which to attach joint springs'
    )
    args = parser.parse_args()

    print("Initializing node... ")
    rospy.init_node("rsdk_joint_torque_springs_%s" % (args.limb,))
    dynamic_cfg_srv = Server(JointSpringsExampleConfig,
                             lambda config, level: config)
    js = JointSprings(args.limb, dynamic_cfg_srv)
    # register shutdown callback
    rospy.on_shutdown(js.clean_shutdown)
    js.move_to_neutral()
    
    js.attach_springs()
    

if __name__ == "__main__":
    main()

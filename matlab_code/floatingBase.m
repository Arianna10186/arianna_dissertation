% create robot
GRETA = GRETA_visualisation('row');

% define floating base
function robot = floatingBaseHelper(df)
    arguments
        df = "column"
    end
robot = rigidBodyTree(DataFormat=df);
robot.BaseName = 'world2';
jointaxname = {'PX','PY','PZ','RX','RY','RZ'};
jointaxval = [eye(3); eye(3)];
parentname = robot.BaseName;
for i = 1:numel(jointaxname)
    bname = ['floating_base_',jointaxname{i}];
    jname = ['floating_base_',jointaxname{i}];
    rb = rigidBody(bname);
    rb.Mass = 0;
    rb.Inertia = zeros(1,6);
    rbjnt = rigidBodyJoint(jname,jointaxname{i}(1));
    rbjnt.JointAxis = jointaxval(i,:);
    rbjnt.PositionLimits = [-inf inf];
    rb.Joint = rbjnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
end

floatingGRETA = floatingBaseHelper("row");
addSubtree(floatingGRETA,"floating_base_RZ",GRETA,ReplaceBase=false);

% export model as a urdf
% exporter = urdfExporter(GRETA);
% writefile(exporter,OutputfileName='GRETA_v3.urdf');

baseOrientation = [pi/6 0 pi]; % ZYX Euler rotation order
basePosition = [1 3 0];
robotZeroConfig = zeros(1,6);
q = [basePosition baseOrientation zeros(1,17)];
%axis equal;
%figure(1)
%show(floatingGRETA,q);
figure(2)
show(GRETA);

%% Forward kinematics for each end effector %%
%%% Front Left Leg %%%
fk_left_front = getTransform(floatingGRETA,floatingGRETA.homeConfiguration,"leg-fl-ee")
%%% Front Right Leg %%%
fk_right_front = getTransform(floatingGRETA,floatingGRETA.homeConfiguration,"leg-fr-ee")
%%% Back Left Leg %%%
fk_left_back = getTransform(floatingGRETA,floatingGRETA.homeConfiguration,"leg-bl-ee")
%%% Back Right Leg %%%
fk_right_back = getTransform(floatingGRETA,floatingGRETA.homeConfiguration,"leg-br-ee")
%%% Tail %%%
fk_tail = getTransform(floatingGRETA,floatingGRETA.homeConfiguration,"tail-ee")
%%% Head %%%
fk_head = getTransform(floatingGRETA,floatingGRETA.homeConfiguration,"head-ee")

% retrieve positions
fl_pos = fk_left_front(1:3,4);
bl_pos = fk_left_back(1:3,4);
fr_pos = fk_right_front(1:3,4);
br_pos = fk_right_back(1:3,4);
%tail_pos = ;
head_pos = fk_head(1:3,4);

% end-effector positions
ee_pos = [fl_pos'; fr_pos'; bl_pos'; br_pos';head_pos'];

%% Inverse kinematics %%
randConfig = GRETA.randomConfiguration;
% show(GRETA,randConfig)

ik = inverseKinematics("RigidBodyTree",GRETA); %ik solution
ik.SolverParameters;

% function ik = iksoln(GRETA)
% ik = inverseKinematics("RigidBodyTree",GRETA);
% ik.SolverParameters;
% end
%ik = iksoln(GRETA);

% get transform for each end effector
trans_fl = getTransform(GRETA,randConfig,'leg-fl-ee');
trans_fr = getTransform(GRETA,randConfig,'leg-fr-ee');
trans_bl = getTransform(GRETA,randConfig,'leg-bl-ee');
trans_br = getTransform(GRETA,randConfig,'leg-br-ee');
trans_head = getTransform(GRETA,randConfig,'head-ee');
trans_tail = getTransform(GRETA,randConfig,'tail-ee');

weights = [0.25, 0.25, 0.25, 1, 1, 1];
initialGuess = GRETA.homeConfiguration;

endEffectors = {'leg-fl-ee', 'leg-fr-ee', 'leg-bl-ee', 'leg-br-ee', 'head-ee'};
desiredTransform = cellfun(@(ee) getTransform(GRETA, randConfig, ee), endEffectors, 'UniformOutput',false);
% [configSoln1,solnInfo1] = ik('leg-fl-ee',trans_fl,weights,initialguess);
% [configSoln2,solnInfo2] = ik('leg-fr-ee',trans_fr,weights,initialguess);
% [configSoln3,solnInfo3] = ik('leg-bl-ee',trans_bl,weights,initialguess);
% [configSoln4,solnInfo4] = ik('leg-br-ee',trans_br,weights,initialguess);
% [configSoln5,solnInfo5] = ik('head-ee',trans_head,weights,initialguess);
%[configSoln6,solnInfo6] = ik('tail-ee',trans_tail,weights,initialguess);

% solve IK iteratively for all ee
configSoln = initialGuess;
for i = 1:length(endEffectors)
    [configSoln, solnInfo] = ik(endEffectors{i}, desiredTransform{i}, weights, configSoln);
    initialGuess = configSoln;
end

disp('Random Joint Configuration: '); disp(randConfig); disp('Joint Configuration Solution: '); disp(configSoln);

% generate c# code
%matlab.engine.typedinterface.generateCSharp("C# code",Functions='iksoln')
%matlab.engine.typedinterface.generateCSharp("C# code",Functions='GRETA_visualisation')

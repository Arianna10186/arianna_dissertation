function robot = GRETA_visualisation(df)
    arguments
        df = 'column'
    end

robot = rigidBodyTree(DataFormat=df);
robot.BaseName = 'world';

% define parameters
jntname_spine_legfl = {'spine-f','shoulder-fl','upleg-fl','leg-fl-ee'};
jntname_legfr = {'shoulder-fr','upleg-fr','leg-fr-ee'};
jntname_spine_legbl = {'spine-b','shoulder-bl','upleg-bl','leg-bl-ee'};
jntname_legbr = {'shoulder-br','upleg-br','leg-br-ee'};
jntname_tail = {'tail-start','tail-ee'};
jntname_head = {'neck-rot','neck-start','head-ee'};

jnttype_fsl = {'fixed','revolute','revolute','fixed'}; % change 1st to revolute, make sure correct rotation axis
jnttype_leg = {'revolute','revolute','fixed'};
jnttype_bsl = {'fixed','revolute','revolute','fixed'};
jnttype_tail = {'fixed','fixed'};
jnttype_head = {'revolute','revolute','fixed'};

poslim_low_fsl = {0,-pi/3,0,0};
poslim_up_fsl = {0,pi/3,pi/3,0};

poslim_low_l = {-pi/3,0,0};
poslim_up_l = {-pi/3,0,0};

poslim_low_bsl = {0,-pi/3,0,0};
poslim_up_bsl = {0,-pi/3,0,0};

poslim_low_tail = {0,0};
poslim_up_tail = {0,0};
poslim_low_head = {0,-pi/2,-pi,0};
poslim_up_head = {0,pi/6,pi,0};

head_homepos = {-pi/4,pi}; %pi/2
bsl_homepos = {0,pi/9,-pi/6};
bl_homepos = {pi/9,-pi/6};

spine_leglen = {1.75,1,1.25,1.35};
leglen = {-1,1.25,1.35};
spine_leglenback = {-1.55,1,1.25,1.35};
leglenback = {-1,1.25,1.35};
taillen = {0.5,0.5};
headlen = {0,0,0};
heady = {-0.5,-2,1};

spine_legrot = {[cosd(-90) sind(-90) 0 0; -sind(-90) cosd(-90) 0 0; 0 0 1 0; 0 0 0 1],...
    [cosd(90) 0 sind(90) 0; 0 1 0 0; -sind(90) 0 cosd(90) 0; 0 0 0 1],...
    1,1};
legrot = {[cosd(90) 0 sind(90) 0; 0 1 0 0; -sind(90) 0 cosd(90) 0; 0 0 0 1],...
    1,1};
tailrot = {[cosd(-90) sind(-90) 0 0; -sind(-90) cosd(-90) 0 0; 0 0 1 0; 0 0 0 1],...
    [cosd(90) 0 sind(90) 0; 0 1 0 0; -sind(90) 0 cosd(90) 0; 0 0 0 1],...
    1};
% headrot = {[cosd(90) 0 sind(90) 0; 0 1 0 0; -sind(90) 0 cosd(90) 0; 0 0 0 1],...
%     [cosd(-90) -sind(-90) 0 0; sind(-90) cosd(-90) 0 0; 0 0 1 0; 0 0 0 1],...
%     1, 1,...
%     [1 0 0 0; 0 cosd(90) sind(90) 0; 0 -sind(90) cosd(90) 0; 0 0 0 1]};
headrot = {[cosd(90) 0 sind(90) 0; 0 1 0 0; -sind(90) 0 cosd(90) 0; 0 0 0 1],...
    [cosd(-90) 0 sind(-90) 0; 0 1 0 0; -sind(-90) 0 cosd(-90) 0; 0 0 0 1],...
    1};
%%
% revolute joint at the centre of the robot
body0 = rigidBody('origin');
jnt0 = rigidBodyJoint('jnt0','revolute'); % middle spine joint

body0.Mass = 1;
jnt0.PositionLimits = [-pi/3, pi/3];

translate0 = trvec2tform([0 0 0]); % body is x distance from ground
rot0 = [cosd(-90) -sind(-90) 0 0; sind(-90) cosd(-90) 0 0; 0 0 1 0; 0 0 0 1]; % -90 rot around z
tform0 = translate0 * rot0;
setFixedTransform(jnt0,tform0); 

body0.Joint = jnt0;
addBody(robot,body0,robot.BaseName);

%% mid spine to leg-fl ee
parentname = 'origin';
for i = 1:numel(jntname_spine_legfl)
    bname = [jntname_spine_legfl{i}];
    jname = [jntname_spine_legfl{i},'-jnt'];

    rb = rigidBody(bname);
    jnt = rigidBodyJoint(jname,jnttype_fsl{i});

    rb.Mass = 1; % need to redefine at some point
    % if strcmp('revolute',jnttype_fsl{i})
    %     jnt.PositionLimits = [poslim_low_fsl{i},poslim_up_fsl{i}];
    % end

    translate = trvec2tform([spine_leglen{i} 0 0]);
    rotate = spine_legrot{i};
    tform = translate * rotate;
    setFixedTransform(jnt,tform);

    rb.Joint = jnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
%% leg-fr
parentname = 'spine-f';
for i = 1:numel(jntname_legfr)
    bname = [jntname_legfr{i}];
    jname = [jntname_legfr{i},'-jnt'];

    rb = rigidBody(bname);
    jnt = rigidBodyJoint(jname,jnttype_leg{i});

    rb.Mass = 1; % need to redefine at some point
    % if strcmp('revolute',jnttype_leg{i})
    %     jnt.PositionLimits = [poslim_low_l{i},poslim_up_l{i}];
    % end

    translate = trvec2tform([leglen{i} 0 0]);
    rotate = legrot{i};
    tform = translate * rotate;
    setFixedTransform(jnt,tform);

    rb.Joint = jnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
%% mid spine to leg-bl ee
parentname = 'origin';
for i = 1:numel(jntname_spine_legbl)
    bname = [jntname_spine_legbl{i}];
    jname = [jntname_spine_legbl{i},'-jnt'];

    rb = rigidBody(bname);
    jnt = rigidBodyJoint(jname,jnttype_fsl{i});

    rb.Mass = 1; % need to redefine at some point
    % if strcmp('revolute',jnttype_bsl{i})
    %     jnt.PositionLimits = [poslim_low_bsl{i},poslim_up_bsl{i}];
    % end
    if strcmp('revolute',jnttype_bsl{i})
        jnt.HomePosition = bsl_homepos{i};
        % jnt.PositionLimits = [poslim_low_head{i},poslim_up_head{i}];
    end

    translate = trvec2tform([spine_leglenback{i} 0 0]);
    rotate = spine_legrot{i};
    tform = translate * rotate;
    setFixedTransform(jnt,tform);

    rb.Joint = jnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
%% leg-br
parentname = 'spine-b';
for i = 1:numel(jntname_legbr)
    bname = [jntname_legbr{i}];
    jname = [jntname_legbr{i},'-jnt'];

    rb = rigidBody(bname);
    jnt = rigidBodyJoint(jname,jnttype_leg{i});

    rb.Mass = 1; % need to redefine at some point
    % if strcmp('revolute',jnttype_leg{i})
    %     jnt.PositionLimits = [poslim_low_l{i},poslim_up_l{i}];
    % end
    if strcmp('revolute',jnttype_leg{i})
        jnt.HomePosition = bl_homepos{i};
        % jnt.PositionLimits = [poslim_low_head{i},poslim_up_head{i}];
    end

    translate = trvec2tform([leglenback{i} 0 0]);
    rotate = legrot{i};
    tform = translate * rotate;
    setFixedTransform(jnt,tform);

    rb.Joint = jnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
%% tail
parentname = 'spine-b';
for i = 1:numel(jntname_tail)
    bname = [jntname_tail{i}];
    jname = [jntname_tail{i},'-jnt'];

    rb = rigidBody(bname);
    jnt = rigidBodyJoint(jname,jnttype_tail{i});

    rb.Mass = 1; % need to redefine at some point
    % if strcmp('revolute',jnttype_tail{i})
    %     jnt.PositionLimits = [poslim_low_tail{i},poslim_up_tail{i}];
    % end

    translate = trvec2tform([taillen{i} 0 0]);
    rotate = tailrot{i};
    tform = rotate * translate;
    setFixedTransform(jnt,tform);

    rb.Joint = jnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
%% head
parentname = 'spine-f';
for i = 1:numel(jntname_head)
    bname = [jntname_head{i}];
    jname = [jntname_head{i},'-jnt'];

    rb = rigidBody(bname);
    jnt = rigidBodyJoint(jname,jnttype_head{i});

    rb.Mass = 1; % need to redefine at some point
    if strcmp('revolute',jnttype_head{i})
        jnt.HomePosition = head_homepos{i};
        % jnt.PositionLimits = [poslim_low_head{i},poslim_up_head{i}];
    end

    translate = trvec2tform([headlen{i} heady{i} 0]);
    rotate = headrot{i};
    tform = rotate * translate;
    setFixedTransform(jnt,tform);

    rb.Joint = jnt;
    robot.addBody(rb,parentname);
    parentname = rb.Name;
end
end
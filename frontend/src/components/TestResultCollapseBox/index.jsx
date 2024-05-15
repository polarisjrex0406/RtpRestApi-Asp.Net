import { useCallback, useEffect, useContext, useState } from 'react';

import {
    EyeOutlined,
    EditOutlined,
    DeleteOutlined,
    EllipsisOutlined,
    RedoOutlined,
    ArrowRightOutlined,
    ArrowLeftOutlined,
    ExperimentOutlined,
    DashboardOutlined
} from '@ant-design/icons';
import { Dropdown, Table, Button, Input, Divider, Spin, Collapse, Tabs, Alert, FloatButton } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';

import { useSelector, useDispatch } from 'react-redux';
import { crud } from '@/redux/crud/actions';
import { selectListItems } from '@/redux/crud/selectors';
import useLanguage from '@/locale/useLanguage';
import { dataForTable } from '@/utils/dataStructure';
import { useDate } from '@/settings';

import { generate as uniqueId } from 'shortid';

import { useCrudContext } from '@/context/crud';
import { selectLangDirection } from '@/redux/translate/selectors';

import Card from '@/components/TestResultCardBox/Card';
import { Tag, Row, Col, Descriptions, Select } from 'antd';
import { useNavigate } from 'react-router-dom';
import { erp } from '@/redux/erp/actions';
import { current } from '@reduxjs/toolkit';
import create from '@ant-design/icons/lib/components/IconFont';

function AddNewItem({ config }) {
    const { crudContextAction } = useCrudContext();
    const { collapsedBox, panel } = crudContextAction;
    const { ADD_NEW_ENTITY } = config;

    const navigate = useNavigate();

    const handelClick = () => {
        navigate(`/test/create`);
    };

    return (
        <Button onClick={handelClick} type="primary">
            {ADD_NEW_ENTITY}
        </Button>
    );
}

const SystemMessage = ({ role, content, order }) => {
    return (
        <>
            <Col key={'3Col' + order} className="gutter-row" span={3} />
            <Col key={'Col18' + order} className="gutter-row" span={18}>
                <Alert
                    key={'Alert' + order}
                    message="Initial Prompt"
                    description={content}
                    type="error"
                />
            </Col>
            <Col key={'Col3' + order} className="gutter-row" span={3} />
            <Divider />
        </>
    );
}

const UserMessage = ({ role, content, order }) => {
    return (
        <>
            <Col key={'Col6' + order} className="gutter-row" span={6} />
            <Col key={'Col17' + order} className="gutter-row" span={17}>
                <Alert
                    key={'Alert' + order}
                    message="Refined Prompt"
                    description={content}
                    type="info"
                />
            </Col>
            <Col key={'Col1' + order} className="gutter-row" span={1} />
            <Divider dashed />
        </>
    );
}

const AssistMessage = ({ role, content, order }) => {
    return (
        <>
            <Col key={'Col1' + order} className="gutter-row" span={1} />
            <Col key={'Col17' + order} className="gutter-row" span={17}>
                <Alert
                    key={'Alert' + order}
                    message="Response"
                    description={content}
                    type="success"
                />
            </Col>
            <Col key={'Col6' + order} className="gutter-row" span={6} />
            <Divider />
        </>
    );
}

const PromptItem = ({ role, content, order }) => {
    if (role === 'system') {
        return (
            <SystemMessage key={role + order} content={content} order={role + order} />
        );
    }
    else if (role === 'user') {
        return (
            <UserMessage key={role + order} content={content} order={role + order} />
        );
    }
    else if (role === 'assistant') {
        return (
            <AssistMessage key={role + order} content={content} order={role + order} />
        );
    }
    else {
        return;
    }
}

const TestItemPaneController = ({ description, handlePrev, handleNext }) => {
    return <Row gutter={[12, 0]} style={{ display: 'flex', justifyContent: 'space-between' }}>
        <Col key={`${uniqueId()}`} className="gutter-row" span={1} />
        <Col key={`${uniqueId()}`} className="gutter-row" span={18}>
            <Descriptions key={`${uniqueId()}`} title={description} />
        </Col>
        <Col key={`${uniqueId()}`} className="gutter-row" span={4} style={{ display: 'flex', justifyContent: 'flex-end' }}>
            <Button
                type="default"
                icon={<ArrowLeftOutlined />}
                onClick={handlePrev}
                style={{ marginRight: '4px' }}
            />
            <Button
                type="default"
                icon={<ArrowRightOutlined />}
                onClick={handleNext}
            />
        </Col>
        <Col key={`${uniqueId()}`} className="gutter-row" span={1} />
    </Row>;
}

const TestItemPaneHeader = ({ currentErp, showMode, handlePrevExp, handleNextExp, handlePrevArti, handleNextArti, handlePrevPrompt, handleNextPrompt }) => {
    let formattedDateTime = '';
    if (currentErp?.created) {
        const dateString = currentErp?.created;
        // Create a new Date object from the input string
        const date = new Date(dateString);
        // Extract the desired date and time components
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        const seconds = String(date.getSeconds()).padStart(2, '0');
        // Construct the desired output string
        formattedDateTime = `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
    }

    return (
        <>
            {
                showMode === 1 ? <TestItemPaneController
                    key={`${uniqueId()}`}
                    description={`${currentErp?.experiment} [${formattedDateTime}]`}
                    handlePrev={handlePrevExp}
                    handleNext={handleNextExp}
                />
                    : <TestItemPaneController
                        key={`${uniqueId()}`}
                        description={`Initial Prompt: ${currentErp?.topicPrompt}`}
                        handlePrev={handlePrevPrompt}
                        handleNext={handleNextPrompt}
                    />
            }
            <TestItemPaneController
                key={`${uniqueId()}`}
                description={`Artifact: ${currentErp?.artifact}`}
                handlePrev={handlePrevArti}
                handleNext={handleNextArti}
            />
        </>
    );
}

const TestItemPaneBody = ({ responses, showMode }) => {
    return (
        <Row gutter={[12, 0]}>
            {responses && responses?.map((resp, index) => (
                <PromptItem key={'PromptItem' + index} role={resp?.role} content={resp?.content} order={index} />
            ))}
        </Row>
    );
}

const TestItemPaneFooter = ({ currentErp, showMode }) => {
    return <>
    </>;
}

const TestItemPane = ({ currentErp, showMode, handlePrevExp, handleNextExp, handlePrevArti, handleNextArti, handlePrevPrompt, handleNextPrompt }) => {
    return <>
        <TestItemPaneHeader
            handlePrevExp={handlePrevExp}
            handleNextExp={handleNextExp}
            handlePrevArti={handlePrevArti}
            handleNextArti={handleNextArti}
            handlePrevPrompt={handlePrevPrompt}
            handleNextPrompt={handleNextPrompt}
            currentErp={currentErp}
            showMode={showMode}
        />
        <TestItemPaneBody responses={currentErp?.responses} />
        <TestItemPaneFooter />
    </>;
}

const prevExp = (fullSource, topicName, curIndex) => {
    let prevIndex = -1;
    for (let i = curIndex + 1; i < fullSource?.length; i++) {
        let currentErp = fullSource[i];
        if (currentErp != null) {
            if (currentErp?.topicName === topicName) {
                prevIndex = i;
                break;
            }
        }
    }
    return prevIndex;
}

const nextExp = (fullSource, topicName, curIndex) => {
    let nextIndex = curIndex;
    for (let i = curIndex - 1; i >= 0; i--) {
        let currentErp = fullSource[i];
        if (currentErp != null) {
            if (currentErp?.topicName === topicName) {
                nextIndex = i;
                break;
            }
        }
    }
    return nextIndex;
}

const prevPrompt = (fullSource, expIndex, curPrompt) => {
    let resPrompt = curPrompt;
    for (const i in fullSource[expIndex]?.experiments[0]?.responses) {
        if (fullSource[expIndex]?.experiments[0]?.responses[i].initPrompt === curPrompt) {
            if (i > 0) {
                resPrompt = fullSource[expIndex]?.experiments[0]?.responses[parseInt(i) - 1]?.initPrompt;
                break;
            }
        }
    }
    return resPrompt;
}

const nextPrompt = (fullSource, expIndex, curPrompt) => {
    let resPrompt = curPrompt;
    for (const i in fullSource[expIndex]?.experiments[0]?.responses) {
        if (fullSource[expIndex]?.experiments[0]?.responses[i].initPrompt === curPrompt) {
            if (i < fullSource[expIndex]?.experiments[0]?.responses.length - 1) {
                resPrompt = fullSource[expIndex]?.experiments[0]?.responses[parseInt(i) + 1]?.initPrompt;
                break;
            }
        }
    }
    return resPrompt;
}

const getResponses = (experiment, artiIndex, topicPrompt) => {
    let responses = [];
    if (!experiment || !Array.isArray(experiment.responses)) {
        return responses; // or return a fallback value
    }

    for (const i in experiment.responses) {
        if (experiment.responses[i]?.initPrompt === topicPrompt) {
            let chatHistory = null;
            if (experiment?.style === 'Stand-alone') {
                chatHistory = { ...experiment?.responses[i]?.chatHistory[artiIndex] };
            } else {
                chatHistory = { ...experiment?.responses[i]?.chatHistory?.slice(-1)[0] };
            }
            responses = [...(chatHistory?.input || []), chatHistory?.output];

            break;
        }
    }
    return responses;
};


const getValuesForPane = (fullSource, expIndex, artiIndex, topicPrompt) => {
    let experiment = fullSource[expIndex]?.experiments[0];
    if (!experiment || !Array.isArray(experiment?.responses)) {
        return {
            experiment: '',
            style: '',
            artifact: '',
            responses: [],
            created: '',
            topicPrompt: '',
        };
    }

    let promptOrd = 0;
    for (const i in experiment.responses) {
        if (topicPrompt === experiment.responses[i].initPrompt) {
            promptOrd = i;
            break;
        }
    }

    let paneValues = {
        experiment: experiment?.experimentCode,
        style: experiment?.style,
        artifact:
            experiment?.style === 'Stand-alone'
                ? experiment?.responses[promptOrd]?.chatHistory[artiIndex]?.artifactName
                : '',
        responses: getResponses(experiment, artiIndex, topicPrompt),
        created: fullSource[expIndex]?.created,
        topicPrompt: topicPrompt,
    };
    return paneValues;
};

const getIndexesForRightPane = (leftInit, showMode, fullSource, expIndex, artiIndex, topicName, topicPrompt) => {
    let newRightIndexes = {
        expIndex: expIndex,
        artiIndex: artiIndex,
        topicPrompt: topicPrompt
    };
    if (showMode === 1) {
        let artifactCount = fullSource[expIndex]?.experiments[0]?.responses[0]?.chatHistory?.length;
        let prevExpIndex = prevExp(fullSource, topicName, expIndex);

        if (leftInit?.style === 'Stand-alone' && artifactCount > artiIndex + 1) {
            newRightIndexes = {
                expIndex: expIndex,
                artiIndex: artiIndex + 1,
                topicPrompt: topicPrompt
            }
        }
        else {
            if (prevExpIndex === -1) {
                newRightIndexes = {
                    expIndex: -1,
                    artiIndex: -1,
                    topicPrompt: ''
                }
            }
            else {
                if (fullSource[prevExpIndex]?.experiments[0]?.style === 'Stand-alone') {
                    newRightIndexes = {
                        expIndex: prevExpIndex,
                        artiIndex: artiIndex,
                        topicPrompt: topicPrompt
                    }
                }
                else {
                    newRightIndexes = {
                        expIndex: prevExpIndex,
                        artiIndex: -1,
                        topicPrompt: topicPrompt
                    }
                }
            }
        }
    }
    else {
        let promptCount = fullSource[expIndex]?.topicPrompt?.length;
        if (promptCount <= artiIndex + 1) {
            newRightIndexes = {
                expIndex: -1,
                artiIndex: -1,
                topicPrompt: ''
            }
        }
        else {
            if (fullSource[expIndex]?.experiments[0]?.style === 'Stand-alone') {
                let artifactCount = fullSource[expIndex]?.experiments[0]?.responses[0]?.chatHistory?.length;
                if (artifactCount > artiIndex + 1) {
                    newRightIndexes = {
                        expIndex: expIndex,
                        artiIndex: artiIndex + 1,
                        topicPrompt: topicPrompt
                    }
                }
                else {
                    newRightIndexes = {
                        expIndex: expIndex,
                        artiIndex: artiIndex,
                        topicPrompt: fullSource[expIndex]?.experiments[0]?.responses[1]?.initPrompt
                    }
                }
            }
            else {
                newRightIndexes = {
                    expIndex: expIndex,
                    artiIndex: -1,
                    topicPrompt: fullSource[expIndex]?.experiments[0]?.responses[1]?.initPrompt
                }
            }
        }
    }

    return newRightIndexes;
}

const TestItem = ({ fullSource, defaultIndex, showMode, topicPrompt, topicName }) => {
    const [expIndexLeft, setExpIndexLeft] = useState(defaultIndex);
    const [artiIndexLeft, setArtiIndexLeft] = useState(0);
    const [promptLeft, setPromptLeft] = useState(topicPrompt);

    const [expIndexRight, setExpIndexRight] = useState(defaultIndex);
    const [artiIndexRight, setArtiIndexRight] = useState(0);
    const [promptRight, setPromptRight] = useState(topicPrompt);

    // Initialize default values
    const [leftOne, setLeftOne] = useState({});
    const [rightOne, setRightOne] = useState({});

    const handlePrevExpLeft = (e) => {
        let nextIndex = nextExp(fullSource, topicName, expIndexLeft);
        if (expIndexLeft !== nextIndex) {
            setExpIndexLeft(nextIndex);
            setArtiIndexLeft(0);

            let leftInit = getValuesForPane(fullSource, nextIndex, 0, promptLeft);
            let rightPaneIndexes = getIndexesForRightPane(leftInit, showMode, fullSource, nextIndex, 0, topicName, promptRight);
            setExpIndexRight(rightPaneIndexes.expIndex);
            setArtiIndexRight(rightPaneIndexes.artiIndex);
        }
    };
    const handleNextExpLeft = (e) => {
        let prevIndex = prevExp(fullSource, topicName, expIndexLeft);
        if (-1 !== prevIndex) {
            setExpIndexLeft(prevIndex);
            setArtiIndexLeft(0);

            let leftInit = getValuesForPane(fullSource, prevIndex, 0, promptLeft);
            let rightPaneIndexes = getIndexesForRightPane(leftInit, showMode, fullSource, prevIndex, 0, topicName, promptRight);
            setExpIndexRight(rightPaneIndexes.expIndex);
            setArtiIndexRight(rightPaneIndexes.artiIndex);
        }
    };
    const handlePrevArtiLeft = (e) => {
        if (artiIndexLeft > 0) {
            setArtiIndexLeft(artiIndexLeft - 1);
        }
    };
    const handleNextArtiLeft = (e) => {
        if (artiIndexLeft < fullSource[expIndexLeft]?.experiments[0]?.responses[0]?.chatHistory?.length - 1) {
            setArtiIndexLeft(artiIndexLeft + 1);
        }
    };
    const handlePrevPromptLeft = (e) => {
        const resPrompt = prevPrompt(fullSource, expIndexLeft, promptLeft);
        setPromptLeft(resPrompt);
    };
    const handleNextPromptLeft = (e) => {
        const resPrompt = nextPrompt(fullSource, expIndexLeft, promptLeft);
        setPromptLeft(resPrompt);
    };

    const handlePrevExpRight = (e) => {
        let nextIndex = nextExp(fullSource, topicName, expIndexRight);
        if (expIndexRight !== nextIndex) {
            setExpIndexRight(nextIndex);
            setArtiIndexRight(0);
        }
    };
    const handleNextExpRight = (e) => {
        let prevIndex = prevExp(fullSource, topicName, expIndexRight);
        if (-1 !== prevIndex) {
            setExpIndexRight(prevIndex);
            setArtiIndexRight(0);
        }
    };
    const handlePrevArtiRight = (e) => {
        if (artiIndexRight > 0) {
            setArtiIndexRight(artiIndexRight - 1);
        }
    };
    const handleNextArtiRight = (e) => {
        if (artiIndexRight < fullSource[expIndexRight]?.experiments[0]?.responses[0]?.chatHistory?.length - 1) {
            setArtiIndexRight(artiIndexRight + 1);
        }
    };
    const handlePrevPromptRight = (e) => {
        const resPrompt = prevPrompt(fullSource, expIndexRight, promptRight);
        setPromptRight(resPrompt);
    };
    const handleNextPromptRight = (e) => {
        const resPrompt = nextPrompt(fullSource, expIndexRight, promptRight);
        setPromptRight(resPrompt);
    };

    useEffect(() => {
        let initPrompt = showMode === 1 ? topicPrompt : fullSource[defaultIndex]?.experiments[0]?.responses[0]?.initPrompt;
        setPromptLeft(initPrompt);
        let leftInit = getValuesForPane(fullSource, defaultIndex, 0, initPrompt);
        const rightPaneIndexes = getIndexesForRightPane(leftInit, showMode, fullSource, defaultIndex, 0, topicName, initPrompt);
        setExpIndexRight(rightPaneIndexes.expIndex);
        setArtiIndexRight(rightPaneIndexes.artiIndex);
        setPromptRight(showMode === 1 ? topicPrompt : rightPaneIndexes.topicPrompt);
    }, [showMode, topicName, topicPrompt]);

    useEffect(() => {
        let leftInit = getValuesForPane(fullSource, expIndexLeft, artiIndexLeft, promptLeft);
        let rightInit = getValuesForPane(fullSource, expIndexRight, artiIndexRight, promptRight);
        setLeftOne(leftInit);
        setRightOne(rightInit);
    }, [promptLeft, promptRight, expIndexLeft, artiIndexLeft, expIndexRight, artiIndexRight]);

    return <>
        <Row gutter={[12, 12]}>
            <Col
                className="gutter-row"
                xs={{ span: 24 }}
                sm={{ span: 12 }}
                md={{ span: 12 }}
                lg={{ span: 12 }}
            >
                <TestItemPane
                    key={`${uniqueId()}`}
                    showMode={showMode}
                    currentErp={leftOne}
                    handlePrevExp={handlePrevExpLeft}
                    handleNextExp={handleNextExpLeft}
                    handlePrevArti={handlePrevArtiLeft}
                    handleNextArti={handleNextArtiLeft}
                    handlePrevPrompt={handlePrevPromptLeft}
                    handleNextPrompt={handleNextPromptLeft}
                />
            </Col>
            <Col
                className="gutter-row"
                xs={{ span: 24 }}
                sm={{ span: 12 }}
                md={{ span: 12 }}
                lg={{ span: 12 }}
            >
                <TestItemPane
                    key={`${uniqueId()}`}
                    showMode={showMode}
                    currentErp={rightOne}
                    handlePrevExp={handlePrevExpRight}
                    handleNextExp={handleNextExpRight}
                    handlePrevArti={handlePrevArtiRight}
                    handleNextArti={handleNextArtiRight}
                    handlePrevPrompt={handlePrevPromptRight}
                    handleNextPrompt={handleNextPromptRight}
                />
            </Col>
        </Row>
    </>;
}

const FilterTest = ({ handleTopicChange, handleTopicPromptChange, topics, topicPrompts, showMode, topicName, topicPrompt }) => {
    const [currentTopicValue, setCurrentTopicValue] = useState('');
    const [currentTopicPromptValue, setCurrentTopicPromptValue] = useState('');

    useEffect(() => {
        const optionsTopic = topics.map((topic) => ({
            value: topic.name,
            label: topic.name,
        }));
        const selectedTopic = optionsTopic.find((option) => option.value === topicName);
        const defaultTopicValue = selectedTopic
            ? selectedTopic.value
            : (optionsTopic[0] ? optionsTopic[0].value : '');

        setCurrentTopicValue(defaultTopicValue);

        const optionsTopicPrompts = topicPrompts.map((prompt) => ({
            value: prompt,
            label: prompt,
        }));
        const selectedPrompt = optionsTopicPrompts.find((option) => option.value === topicPrompt);
        const defaultPromptValue = selectedPrompt
            ? selectedPrompt.value
            : (optionsTopicPrompts[0] ? optionsTopicPrompts[0].value : '');

        setCurrentTopicPromptValue(defaultPromptValue);
    }, [topics, topicPrompts, topicName, topicPrompt]);

    return (
        <>
            <Row gutter={[12, 12]}>
                <Col
                    className="gutter-row"
                    xs={{ span: 24 }}
                    sm={{ span: 24 }}
                    md={{ span: 24 }}
                    lg={{ span: 24 }}
                >
                    {/* <Button
                        type="default"
                        icon={<ArrowLeftOutlined />}
                        style={{
                            width: '5%',
                        }}
                        disabled={showMode === 2}
                    /> */}
                    <Select
                        value={currentTopicValue}
                        style={{
                            width: '100%',
                        }}
                        options={topics.map((topic) => ({
                            value: topic.name,
                            label: topic.name,
                        }))}
                        onChange={handleTopicChange}
                        disabled={showMode === 2}
                    />
                    {/* <Button
                        type="default"
                        icon={<ArrowRightOutlined />}
                        style={{
                            width: '5%',
                        }}
                        disabled={showMode === 2}
                    /> */}
                </Col>
                <Col
                    className="gutter-row"
                    xs={{ span: 24 }}
                    sm={{ span: 24 }}
                    md={{ span: 24 }}
                    lg={{ span: 24 }}
                >
                    {/* <Button
                        type="default"
                        icon={<ArrowLeftOutlined />}
                        style={{
                            width: '5%',
                        }}
                        disabled={showMode === 2}
                    /> */}
                    <Select
                        value={currentTopicPromptValue}
                        style={{
                            width: '100%',
                        }}
                        options={topicPrompts.map((prompt) => ({
                            value: prompt,
                            label: prompt,
                        }))}
                        onChange={handleTopicPromptChange}
                        disabled={showMode === 2}
                    />
                    {/* <Button
                        type="default"
                        icon={<ArrowRightOutlined />}
                        style={{
                            width: '5%',
                        }}
                        disabled={showMode === 2}
                    /> */}
                </Col>
            </Row>
        </>
    );
};


export default function TestResultCollapseBox({ config, extra = [] }) {
    const filterText = useContext('FilterTextContext');

    let { entity, dataTableColumns, DATATABLE_TITLE, fields, searchConfig } = config;
    const { crudContextAction } = useCrudContext();
    const { panel, collapsedBox, modal, readBox, editBox, advancedBox } = crudContextAction;
    const translate = useLanguage();
    const { dateFormat } = useDate();

    const items = [
        {
            label: translate('Show'),
            key: 'read',
            icon: <EyeOutlined />,
        },
        {
            label: translate('Edit'),
            key: 'edit',
            icon: <EditOutlined />,
        },
        ...extra,
        {
            type: 'divider',
        },

        {
            label: translate('Delete'),
            key: 'delete',
            icon: <DeleteOutlined />,
        },
    ];

    function handleDelete(record) {
        dispatch(crud.currentAction({ actionType: 'delete', data: record }));
        modal.open();
    }

    const { result: listResult, isLoading: listIsLoading } = useSelector(selectListItems);

    const { pagination, items: dataSource } = listResult;

    const dispatch = useDispatch();

    const handelDataTableLoad = useCallback((pagination) => {
        const options = { page: pagination.current || 1, items: pagination.pageSize || 10 };
        dispatch(crud.list({ entity, options }));
    }, []);

    const filterTable = (e) => {
        const value = e.target.value;
        const options = { q: value, fields: searchConfig?.searchFields || '' };
        dispatch(crud.list({ entity, options }));
    };

    const dispatcher = () => {
        dispatch(crud.list({ entity }));
    };

    useEffect(() => {
        const controller = new AbortController();
        dispatcher();
        return () => {
            controller.abort();
        };
    }, []);

    const langDirection = useSelector(selectLangDirection);

    const [topicName, setTopicName] = useState('');
    const [topicPrompts, setTopicPrompts] = useState([]);
    const [topics, setTopics] = useState([]);
    const [initPrompt, setInitPrompt] = useState('');
    const [showMode, setShowMode] = useState(1);

    const handleTopicChange = (e) => {
        setTopicName(e);
        const selectedTopic = topics.find(item => item.name === e);
        setTopicPrompts([...selectedTopic.prompts, '']);
    };
    const handleTopicPromptChange = (e) => {
        setInitPrompt(e);
    }
    const handleTestChange = (e) => {
        const sortedDataSource = [...dataSource].sort((a, b) => new Date(b.created) - new Date(a.created));
        if (e != null && e.length > 0) {
            let index = parseInt(e[0]) - 1;
            setTopicName(sortedDataSource[index]?.topicName);
            const selectedTopic = topics?.find(item => item?.name === sortedDataSource[index]?.topicName);
            setTopicPrompts([...selectedTopic?.prompts, '']);
        }
        else {
            setTopicName('');
            setTopicPrompts([]);
            setInitPrompt('');
        }
    }
    const handleShowModeChange = (e) => {
        if (e === 1 && dataSource?.length > 0) {
            const sortedDataSource = [...dataSource].sort((a, b) => new Date(b.created) - new Date(a.created));
            let tmp = [];
            for (const i in sortedDataSource) {
                const isDup = tmp.some(item => item.name === sortedDataSource[i].topicName);
                if (!isDup) {
                    tmp.push({
                        name: sortedDataSource[i].topicName,
                        prompts: sortedDataSource[i].topicPrompt
                    });
                }
            }
            setTopics(tmp);
            setTopicName(sortedDataSource[0]?.topicName);
            const selectedTopic = tmp.find(item => item.name === sortedDataSource[0]?.topicName);
            setTopicPrompts([...selectedTopic?.prompts, '']);
            setInitPrompt(sortedDataSource[0]?.experiments[0]?.responses[0]?.initPrompt);
        }
        else {
            setTopicName('');
            setTopicPrompts([]);
            setInitPrompt('');
        }
    }

    useEffect(() => {
        if (dataSource && dataSource.length > 0 && dataSource[0].experiments) {
            const sortedDataSource = [...dataSource].sort((a, b) => new Date(b.created) - new Date(a.created));
            let tmp = [];
            for (const i in sortedDataSource) {
                const isDup = tmp.some(item => item.name === sortedDataSource[i].topicName);
                if (!isDup) {
                    tmp.push({
                        name: sortedDataSource[i].topicName,
                        prompts: sortedDataSource[i].topicPrompt
                    });
                }
            }
            setTopics(tmp);

            if (showMode === 1) {
                setTopicName(sortedDataSource[0].topicName);
                const selectedTopic = tmp.find(item => item.name === sortedDataSource[0].topicName);
                setTopicPrompts([...selectedTopic.prompts, '']);
                setInitPrompt(sortedDataSource[0]?.experiments[0]?.responses[0]?.initPrompt);
            }
        }
    }, [dataSource]);

    const [activeKey, setActiveKey] = useState(1);
    useEffect(() => {
        setActiveKey(1);
    }, [showMode]);

    return (
        <>
            <PageHeader
                onBack={() => window.history.back()}
                backIcon={langDirection === "rtl" ? <ArrowRightOutlined /> : <ArrowLeftOutlined />}
                title={DATATABLE_TITLE}
                ghost={false}
                extra={[
                    <Input
                        key={`searchFilterDataTable}`}
                        onChange={filterTable}
                        placeholder={translate('search')}
                        allowClear
                    />,
                    <Button onClick={handelDataTableLoad} key={`${uniqueId()}`} icon={<RedoOutlined />}>
                        {translate('Refresh')}
                    </Button>,

                    <AddNewItem key={`${uniqueId()}`} config={config} />,
                ]}
                style={{
                    padding: '20px 0px',
                    direction: langDirection,
                    position: 'fixed',
                    top: '0px',
                }}
            ></PageHeader>

            {(() => {
                const testRows = [];
                if (listIsLoading) {
                    return <div className="centerAbsolute"><Spin size="large" /></div>
                }

                return <Row gutter={[32, 32]}>
                    <Col
                        className="gutter-row"
                        xs={{ span: 24 }}
                        sm={{ span: 24 }}
                        md={{ span: 24 }}
                        lg={{ span: 24 }}
                    >
                        <div
                            className="whiteBox shadow"
                            style={{ color: '#595959', fontSize: 13, minHeight: '106px', height: '100%' }}
                        >
                            <FloatButton
                                shape="circle"
                                type={showMode === 1 ? 'primary' : 'default'}
                                style={{
                                    right: 74,
                                }}
                                icon={<ExperimentOutlined />}
                                onClick={() => {
                                    setShowMode(1);
                                    handleShowModeChange(1);
                                }}
                            />
                            <FloatButton
                                shape="circle"
                                type={showMode === 2 ? 'primary' : 'default'}
                                style={{
                                    right: 24,
                                }}
                                icon={<DashboardOutlined />}
                                onClick={() => {
                                    setShowMode(2);
                                    handleShowModeChange(2);
                                }}
                            />
                            {
                                (() => {
                                    const sortedDataSource = [...dataSource].sort((a, b) => new Date(b.created) - new Date(a.created));
                                    if (sortedDataSource.length > 0 && dataSource[0].experiments) {
                                        if (showMode === 2) {
                                            for (const i in sortedDataSource) {
                                                let j = parseInt(i) + 1;

                                                let formattedDateTime = '';
                                                if (sortedDataSource[i]?.created) {
                                                    const dateString = sortedDataSource[i]?.created;
                                                    // Create a new Date object from the input string
                                                    const date = new Date(dateString);
                                                    // Extract the desired date and time components
                                                    const year = date.getFullYear();
                                                    const month = String(date.getMonth() + 1).padStart(2, '0');
                                                    const day = String(date.getDate()).padStart(2, '0');
                                                    const hours = String(date.getHours()).padStart(2, '0');
                                                    const minutes = String(date.getMinutes()).padStart(2, '0');
                                                    const seconds = String(date.getSeconds()).padStart(2, '0');
                                                    // Construct the desired output string
                                                    formattedDateTime = `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
                                                }

                                                testRows.push({
                                                    key: j,
                                                    label: sortedDataSource[i].experiments[0].experimentCode + ' [' + formattedDateTime + ']',
                                                    children: <TestItem
                                                        fullSource={sortedDataSource}
                                                        defaultIndex={i}
                                                        showMode={showMode}
                                                        topicPrompt={initPrompt}
                                                        topicName={topicName}
                                                    />
                                                });
                                            }
                                        }
                                        return <>
                                            {
                                                showMode === 1 ? <FilterTest
                                                    handleTopicChange={handleTopicChange}
                                                    handleTopicPromptChange={handleTopicPromptChange}
                                                    topics={topics}
                                                    topicPrompts={topicPrompts}
                                                    showMode={showMode}
                                                    topicName={topicName}
                                                    topicPrompt={initPrompt}
                                                /> : <div />
                                            }
                                            <Row gutter={[12, 12]}>
                                                <Col
                                                    className="gutter-row"
                                                    xs={{ span: 24 }}
                                                    sm={{ span: 24 }}
                                                    md={{ span: 24 }}
                                                    lg={{ span: 24 }}
                                                >
                                                    {
                                                        showMode === 1 ? <TestItem
                                                            fullSource={sortedDataSource}
                                                            defaultIndex={0}
                                                            showMode={showMode}
                                                            topicPrompt={initPrompt}
                                                            topicName={topicName}
                                                        /> : <Collapse accordion items={testRows} defaultActiveKey={activeKey} onChange={handleTestChange} />
                                                    }
                                                </Col>
                                            </Row>
                                        </>;
                                    }
                                    else {
                                        return <div key={`${uniqueId()}`} style={{ textAlign: 'center', position: 'absolute', top: '50%', left: '50%' }}>
                                            <h1 style={{ display: 'inline' }}>No Results</h1>
                                        </div>;
                                    }
                                })()
                            }
                        </div>
                    </Col>
                </Row >;
            })()}
        </>
    );
}

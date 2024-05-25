import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useSelector, useDispatch } from 'react-redux';

import { Button, Row, Col, Descriptions, Card, Divider } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import {
  EditOutlined,
  CloseCircleOutlined,
} from '@ant-design/icons';

import useLanguage from '@/locale/useLanguage';
import { erp } from '@/redux/erp/actions';
import { generate as uniqueId } from 'shortid';
import { selectCurrentItem } from '@/redux/erp/selectors';


const PromptEnhancerItem = ({ item, currentErp }) => {
  return (
    <Row gutter={[12, 0]} key={item._id}>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.key}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.valueType}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.value}
        </p>
      </Col>
      <Col className="gutter-row" span={6}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.description}
        </p>
      </Col>
      <Col className="gutter-row" span={9}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.promptModifier}
        </p>
      </Col>
    </Row>
  );
};

const ChatGptSettingItem = ({ item, currentErp }) => {
  return (
    <Row gutter={[12, 0]} key={item._id}>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.setting}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.valueType}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.value}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.minValue}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.maxValue}
        </p>
      </Col>
      <Col className="gutter-row" span={9}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.description}
        </p>
      </Col>
    </Row>
  );
};

const CriteriaConditionItem = ({ item, currentErp }) => {
  return (
    <Row gutter={[12, 0]} key={item._id}>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
      <Col className="gutter-row" span={4}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.conditionName}
        </p>
      </Col>
      <Col className="gutter-row" span={5}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.conditionType}
        </p>
      </Col>
      <Col className="gutter-row" span={5}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.conditionItem}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.conditionOperator}
        </p>
      </Col>
      <Col className="gutter-row" span={7}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.conditionValue}
        </p>
      </Col>
    </Row>
  );
}

const CriteriaRuleItem = ({ order, item, currentErp }) => {
  return (
    <>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
      <Card>
        <Row gutter={[12, 0]} key={item._id}>
          <Col className="gutter-row" span={16}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              {item.ruleName}
            </p>
          </Col>
          <Col className="gutter-row" span={8}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              {item.conditionsLogic}
            </p>
          </Col>
          <Col className="gutter-row" span={4}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>Condition Name</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={5}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>Condition Type</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={5}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>Condition Item</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>Condition Operator</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={7}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>Condition Value</strong>
            </p>
          </Col>
        </Row>
        {
          item?.conditions?.map((condition, index) => (
            <CriteriaConditionItem key={'condition' + index} order={index + 1} item={condition} currentErp={currentErp} />
          ))
        }
      </Card>
    </>
  );
};

const CacheConditionItem = ({ item, currentErp }) => {
  return (
    <Row gutter={[12, 0]} key={item._id}>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.key}
        </p>
      </Col>
      <Col className="gutter-row" span={3}>
        <p
          style={{
            textAlign: 'left',
          }}
        >
          {item.changeDetection}
        </p>
      </Col>
      <Divider dashed style={{ marginTop: 0, marginBottom: 15 }} />
    </Row>
  );
};


export default function ReadItem({ config, selectedItem }) {
  const translate = useLanguage();
  const { entity, ENTITY_NAME } = config;
  const dispatch = useDispatch();
  const navigate = useNavigate();

  const { result: currentResult } = useSelector(selectCurrentItem);

  const resetErp = {
    removed: false,
    enabled: true,
    name: '',
    group: '',
    goal: '',
    topic: {
      name: '',
    },
    prompt_output: '',
    useCache: false,
    cacheTimeoutUnit: null,
    cacheTimeoutValue: 0,
    cacheConditions: [],
    description: ''
  };

  const [currentErp, setCurrentErp] = useState(selectedItem ?? resetErp);
  const [promptEnhancers, setPromptEnhancers] = useState([]);

  useEffect(() => {
    if (currentResult) {
      const { promptEnhancerItems, criteriaItems, retentionSettingItems, ...others } = currentResult;

      if (promptEnhancerItems) {
        setPromptEnhancers(promptEnhancerItems);
      }
      setCurrentErp(currentResult);
    }
    return () => {
      setPromptEnhancers([]);
      setCurrentErp(resetErp);
    };
  }, [currentResult]);

  useEffect(() => {
    if (currentErp?.client) {
      setClient(currentErp.client[currentErp.client.type]);
    }
  }, [currentErp]);

  return (
    <>
      <PageHeader
        onBack={() => {
          if (currentErp?.topicId) navigate(`/template/${currentErp?.topicId}`);
        }}
        title={`Artifact`}
        extra={[
          <Button
            key={`${uniqueId()}`}
            onClick={() => {
              if (currentErp?.topicId) navigate(`/template/${currentErp?.topicId}`);
            }}
            icon={<CloseCircleOutlined />}
          >
            {translate('Close')}
          </Button>,
          <Button
            key={`${uniqueId()}`}
            onClick={() => {
              dispatch(
                erp.currentAction({
                  actionType: 'update',
                  data: currentErp,
                })
              );
              navigate(`/${entity.toLowerCase()}/update/${currentErp._id}`);
            }}
            type="primary"
            icon={<EditOutlined />}
          >
            {translate('Edit')}
          </Button>,
        ]}
        style={{
          padding: '20px 0px',
        }}
      >
      </PageHeader>
      <Divider dashed />
      <Descriptions title={`${currentErp.name} - ${currentErp.group}`}>
        <Descriptions.Item label={translate('Goal')}>{currentErp.goal}</Descriptions.Item>
        <Descriptions.Item label={translate('Prompt Output')}>{currentErp.promptOutput}</Descriptions.Item>
      </Descriptions>
      <Divider orientation="right">Prompt Enhancers</Divider>
      <Card>
        <Row gutter={[12, 0]}>
          {/* Key, Value Type, Value, Description, Prompt Modifier */}
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Key')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Value Type')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Value')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={6}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Description')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={9}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Prompt Modifier')}</strong>
            </p>
          </Col>
        </Row>
        {currentErp?.promptEnhancers?.map((item, index) => (
          <PromptEnhancerItem key={'enhanceItem' + index} item={item} currentErp={currentErp} />
        ))}
      </Card>

      <Divider orientation="right">ChatGPT Settings</Divider>
      <Card>
        <Descriptions title='ChatGPT Settings' />
        <Row gutter={[12, 0]}>
          {/* Key, Value Type, Value, Description, Prompt Modifier */}
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Setting')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Value Type')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Value')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Min')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={3}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Max')}</strong>
            </p>
          </Col>
          <Col className="gutter-row" span={9}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              <strong>{translate('Description')}</strong>
            </p>
          </Col>
        </Row>
        {currentErp?.chatgptSettings?.map((item, index) => (
          <ChatGptSettingItem key={'gptSetting' + index} item={item} currentErp={currentErp} />
        ))}
      </Card>

      <Divider orientation="right">Criteria</Divider>
      <Card>
        <Row gutter={[12, 0]}>
          <Col className="gutter-row" span={20}>
            <p
              style={{
                textAlign: 'right',
              }}
            >
              {translate('Rule Logic:')}
            </p>
          </Col>
          <Col className="gutter-row" span={4}>
            <p
              style={{
                textAlign: 'left',
              }}
            >
              {currentErp?.ruleLogic}
            </p>
          </Col>
        </Row>
        {currentErp?.rules?.map((item, index) => (
          <CriteriaRuleItem key={'criteria' + index} order={index + 1} item={item} currentErp={currentErp} />
        ))}
      </Card>

      <Divider orientation="right">Retention Settings</Divider>
      <Card>
        <Descriptions title=''>
          <Descriptions.Item label={translate('Use Cache')}>
            {currentErp?.useCache ? currentErp?.useCache?.toString() : ''}
          </Descriptions.Item>
          <Descriptions.Item label={translate('Cache Timeout Unit')}>
            {currentErp?.cacheTimeoutUnit}
          </Descriptions.Item>
          <Descriptions.Item label={translate('Cache Timeout Value')}>
            {currentErp?.cacheTimeoutValue}
          </Descriptions.Item>
        </Descriptions>
        {currentErp?.useCache && currentErp?.cacheConditions?.map((item, index) => (
          <CacheConditionItem key={'cacheCond' + index} item={item} currentErp={currentErp} />
        ))}
      </Card>
    </>
  );
}

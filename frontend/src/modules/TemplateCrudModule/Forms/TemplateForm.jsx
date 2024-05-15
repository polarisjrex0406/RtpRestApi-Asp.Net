import { useEffect, useRef, useState } from 'react';

import { Form, Input, Button, Select, Divider, Row, Col } from 'antd';

import { PlusOutlined } from '@ant-design/icons';

import AutoCompleteAsync from '@/components/AutoCompleteAsync';

import PromptEnhancerItemRow from '@/modules/TemplateCrudModule/ItemsRow/PromptEnhancerItemRow';
import ChatGptSettingItemRow from '@/modules/TemplateCrudModule/ItemsRow/ChatGptSettingItemRow';
import RetentionSettingItemRow from '@/modules/TemplateCrudModule/ItemsRow/RetentionSettingItemRow';

import useLanguage from '@/locale/useLanguage';

import TextArea from 'antd/es/input/TextArea';

import { request } from '@/request';

export default function TemplateForm({ subTotal = 0, current = null }) {
  return <LoadTemplateForm subTotal={subTotal} current={current} />;
}

const validateUnique = (originName, currentName, oldNames, curTopicId) => {
  let curNames = oldNames;
  if (curTopicId) {
    curNames = curNames.filter(item => item.topicId === curTopicId);
  }
  const count = curTopicId ? curNames.filter(item => item.name === currentName).length : 0;
  if (currentName?.length > 0 && (count === 0 || (count === 1 && originName == currentName))) {
    return {
      validateStatus: 'success',
      errorMsg: null,
    };
  }
  return {
    validateStatus: 'error',
    errorMsg: 'Artifact name must be non-empty and unique under same topic!',
  };
};

function LoadTemplateForm({ subTotal = 0, current = null }) {
  const translate = useLanguage();
  const addField = useRef(false);

  useEffect(() => {
    // addField.current.click();
  }, []);

  const [searching, setSearching] = useState(false);
  const [keyword, setKeyWord] = useState('');
  const [currentName, setCurrentName] = useState({
    value: undefined
  });
  const originName = current ? current.name : null;

  const pathSegments = window.location.pathname.split('/');
  const methodFromUrl = pathSegments[pathSegments.length - 2];
  const chatIdFromUrl = pathSegments[pathSegments.length - 1];

  const [oldNames, setOldNames] = useState([]);
  const [curTopicId, setCurTopicId] = useState(methodFromUrl === 'create' ? chatIdFromUrl : '');

  const asyncSearch = async (entity = 'template') => {
    const options = {};
    setSearching(true);
    const res = await request.list({ entity, options });
    setOldNames(res.result.map(obj => ({ name: obj.name, topicId: obj.topicId })));
    setSearching(false);
    return res;
  };

  useEffect(() => {
    const callback = asyncSearch();
  }, [keyword]);

  const onNameChange = (originName, value, oldNames, curTopicId) => {
    setCurrentName({
      ...validateUnique(originName, value, oldNames, curTopicId),
      value: value,
    });
  };

  return (
    <>
      <Row gutter={[12, 0]}>
        {/* Name, Group, Topic */}
        <Col className="gutter-row" span={8}>
          <Form.Item
            label={translate('name')}
            name="name"
            rules={[
              {
                required: true,
              },
            ]}
            hasFeedback
            validateStatus={currentName.validateStatus}
            help={currentName.errorMsg || ''}
          >
            <Input
              style={{ width: '100%' }}
              disabled={searching}
              placeholder={searching ? '... Loading names' : ''}
              value={currentName.value}
              onChange={(e) => {
                onNameChange(originName, e.target.value, oldNames, curTopicId);
              }}
            />
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={8}>
          <Form.Item
            label={translate('group')}
            name="group"
          >
            <Input style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={8}>
          <Form.Item
            name="topic"
            label={translate('topic')}
          >
            <AutoCompleteAsync
              entity={'topic'}
              displayLabels={['name']}
              searchFields={'name'}
              redirectLabel={'Add New Topic'}
              withRedirect
              urlToRedirect={'/topic'}
              onChange={(e) => {
                setCurTopicId(e);
                if (currentName.value?.length > 0) {
                  setCurrentName({
                    ...validateUnique(originName, currentName.value, oldNames, e),
                    value: currentName.value,
                  });
                }
                else {
                }
              }}
              disabled={true}
              fixedInitValue={methodFromUrl === 'create' ? chatIdFromUrl : ''}
            />
          </Form.Item>
        </Col>
        {/* Goal, Prompt Output */}
        <Col className="gutter-row" span={8}>
          <Form.Item
            label={translate('goal')}
            name="goal"
          >
            <TextArea style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={16}>
          <Form.Item
            label={translate('promptOutput')}
            name="promptOutput"
          >
            <TextArea style={{ width: '100%' }} />
          </Form.Item>
        </Col>
      </Row>

      {/* Prompt Enhancers */}
      <Divider dashed />
      <Row gutter={[12, 12]} style={{ position: 'relative' }}>
        <Col className="gutter-row" span={24}>
          <p>{translate('Prompt Enhancers')}</p>
        </Col>
      </Row>
      <Form.List name="promptEnhancers">
        {(fields, { add, remove }) => (
          <>
            {fields.map((field) => (
              <PromptEnhancerItemRow key={field.key} remove={remove} field={field} current={current}></PromptEnhancerItemRow>
            ))}
            <Form.Item>
              <Button
                type="dashed"
                onClick={() => add()}
                block
                icon={<PlusOutlined />}
                ref={addField}
              >
                {translate('Add Prompt Enhancer field')}
              </Button>
            </Form.Item>
          </>
        )}
      </Form.List>

      {/* ChatGPT Settings */}
      <Divider dashed />
      <Row gutter={[12, 12]} style={{ position: 'relative' }}>
        <Col className="gutter-row" span={24}>
          <p>{translate('ChatGPT Settings')}</p>
        </Col>
      </Row>
      <Form.List name="chatgptSettings">
        {(fields, { add, remove }) => (
          <>
            {fields.map((field) => (
              <ChatGptSettingItemRow key={field.key} remove={remove} field={field} current={current}>
              </ChatGptSettingItemRow>
            ))}
            <Form.Item>
              <Button
                type="dashed"
                onClick={() => add()}
                block
                icon={<PlusOutlined />}
                ref={addField}
              >
                {translate('Add ChatGPT Settings field')}
              </Button>
            </Form.Item>
          </>
        )}
      </Form.List>

      {/* Retention Settings */}
      <Divider dashed />
      <Row gutter={[12, 12]} style={{ position: 'relative' }}>
        <Col className="gutter-row" span={24}>
          <p>{translate('Retention Settings')}</p>
        </Col>
      </Row>
      {/* UseCache, CacheTimeoutUnit, CacheTimeoutValue */}
      <Row gutter={[12, 12]} style={{ position: 'relative' }}>
        <Col className="gutter-row" span={8}>
          <Form.Item
            label={translate('Use cache')}
            name="useCache"
          >
            <Select
              options={[
                { value: true, label: 'true' },
                { value: false, label: 'false' },
              ]}
            ></Select>
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={8}>
          <Form.Item
            label={translate('cacheTimeoutUnit')}
            name="cacheTimeoutUnit"
          >
            <Select
              options={[
                { value: 'seconds', label: 'seconds' },
                { value: 'minutes', label: 'minutes' },
                { value: 'hours', label: 'hours' },
                { value: 'days', label: 'days' },
                { value: 'weeks', label: 'weeks' },
                { value: 'months', label: 'months' },
              ]}
            ></Select>
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={8}>
          <Form.Item
            label={translate('cacheTimeoutValue')}
            name="cacheTimeoutValue"
          >
            <Input placeholder="Cache Timeout Value" />
          </Form.Item>
        </Col>
        {/* Description */}
        <Col className="gutter-row" span={24}>
          <Form.Item
            label={translate('cacheDescription')}
            name="cacheDescription"
          >
            <TextArea placeholder="Description" />
          </Form.Item>
        </Col>
      </Row>
      {/* Setting Conditions */}
      <Form.List name="cacheConditions">
        {(fields, { add, remove }) => (
          <>
            {fields.map((field) => (
              <RetentionSettingItemRow key={field.key} remove={remove} field={field} current={current}>
              </RetentionSettingItemRow>
            ))}
            <Form.Item>
              <Button
                type="dashed"
                onClick={() => add()}
                block
                icon={<PlusOutlined />}
                ref={addField}
              >
                {translate('Add Retention Conditions field')}
              </Button>
            </Form.Item>
          </>
        )}
      </Form.List>
      <div style={{ position: 'relative', width: ' 100%', float: 'right' }}>
        <Row gutter={[12, -5]}>
          <Col className="gutter-row" span={5}>
            <Form.Item>
              <Button type="primary" htmlType="submit" icon={<PlusOutlined />} block>
                {translate('Save')}
              </Button>
            </Form.Item>
          </Col>
        </Row>
      </div>
    </>
  );
}

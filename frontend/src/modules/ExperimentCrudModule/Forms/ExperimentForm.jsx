import { useEffect, useRef } from 'react';

import { Form, Input, Button, Select, Divider, Row, Col } from 'antd';
import TextArea from 'antd/es/input/TextArea';
import { PlusOutlined } from '@ant-design/icons';

import useLanguage from '@/locale/useLanguage';

import AutoCompleteAsync from '@/components/AutoCompleteAsync';
import TemplateItemRow from '@/modules/ExperimentCrudModule/ItemsRow/TemplateItemRow'
import InitPromptItemRow from '@/modules/ExperimentCrudModule/ItemsRow/InitPromptItemRow';

export default function ExperimentForm({ subTotal = 0, current = null, handleTopicChange, curTopicId }) {
  return <LoadExperimentForm subTotal={subTotal} current={current} handleTopicChange={handleTopicChange} curTopicId={curTopicId} />;
}

function LoadExperimentForm({ subTotal = 0, current = null, handleTopicChange, curTopicId }) {
  const translate = useLanguage();
  const addArtifactField = useRef(false);
  const addInitPromptField = useRef(false);

  return (
    <>
      <Row gutter={[12, 0]}>
        {/* experimentCode, style, topic */}
        <Col className="gutter-row" span={10}>
          <Form.Item
            label={translate('experimentCode')}
            name="experimentCode"
            rules={[
              {
                required: true,
              },
            ]}
          >
            <Input style={{ width: '100%' }} />
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={4}>
          <Form.Item
            label={translate('style')}
            name="style"
            rules={[
              {
                required: true,
              },
            ]}
          >
            <Select
              options={[
                { value: 'Stand-alone', label: 'Stand-alone' },
                { value: 'Conversation', label: 'Conversation' },
              ]}
            ></Select>
          </Form.Item>
        </Col>
        <Col className="gutter-row" span={10}>
          <Form.Item
            name="topic"
            label={translate('topic')}
            rules={[
              {
                required: true,
              },
            ]}
          >
            <AutoCompleteAsync
              entity={'topic'}
              displayLabels={['name']}
              searchFields={'name'}
              redirectLabel={'Add New Topic'}
              withRedirect
              urlToRedirect={'/topic'}
              onChange={handleTopicChange}
            />
          </Form.Item>
        </Col>
        {/* description */}
        <Col className="gutter-row" span={24}>
          <Form.Item
            label={translate('description')}
            name="description"
          >
            <TextArea style={{ width: '100%' }} />
          </Form.Item>
        </Col>
      </Row>
      {/* initPrompt */}
      <Divider dashed />
      <Row gutter={[12, 12]} style={{ position: 'relative' }}>
        <Col className="gutter-row" span={24}>
          <p>{translate('Init Prompts')}</p>
        </Col>
      </Row>
      <Form.List name="initPrompt">
        {(fields, { add, remove }) => (
          <>
            {fields.map((field) => (
              <InitPromptItemRow key={field.key} remove={remove} field={field} current={current} curTopicId={curTopicId} />
            ))}
            {curTopicId && (
              <Form.Item>
                <Button
                  type="dashed"
                  onClick={() => {
                    add();
                  }}
                  block
                  icon={<PlusOutlined />}
                  ref={addInitPromptField}
                >
                  {translate('Add init prompt field')}
                </Button>
              </Form.Item>
            )}
          </>
        )}
      </Form.List>

      {/* Templates */}
      <Divider dashed />
      <Row gutter={[12, 12]} style={{ position: 'relative' }}>
        <Col className="gutter-row" span={24}>
          <p>{translate('Artifacts')}</p>
        </Col>
      </Row>
      <Form.List name="templates">
        {(fields, { add, remove }) => (
          <>
            {fields.map((field) => (
              <TemplateItemRow key={field.key} remove={remove} field={field} current={current} curTopicId={curTopicId} />
            ))}
            {curTopicId && (
              <Form.Item>
                <Button
                  type="dashed"
                  onClick={() => {
                    add();
                  }}
                  block
                  icon={<PlusOutlined />}
                  ref={addArtifactField}
                >
                  {translate('Add artifacts field')}
                </Button>
              </Form.Item>
            )}
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

import { useState, useEffect } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate, useParams } from 'react-router-dom';
import { generate as uniqueId } from 'shortid';

import { Form, Divider, Button } from 'antd';
import { PageHeader } from '@ant-design/pro-layout';
import { CloseCircleOutlined, PlusOutlined } from '@ant-design/icons';

import { erp } from '@/redux/erp/actions';
import { selectUpdatedItem } from '@/redux/erp/selectors';
import Loading from '@/components/Loading';
import useLanguage from '@/locale/useLanguage';

function SaveForm({ form, translate }) {
  const handelClick = () => {
    form.submit();
  };
  return (
    <Button onClick={handelClick} type="primary" icon={<PlusOutlined />}>
      {translate('update')}
    </Button>
  );
}

export default function UpdateItem({ config, UpdateForm }) {
  const translate = useLanguage();
  let { entity } = config;
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { current, isLoading, isSuccess } = useSelector(selectUpdatedItem);
  const [form] = Form.useForm();
  const resetErp = {
    removed: false,
    enabled: true,
    experimentCode: '',
    description: '',
    style: 'Stand-alone',
    topic: {},
    templates: [],
  };
  const [currentErp, setCurrentErp] = useState(current ?? resetErp);
  const { id } = useParams();
  const [curTopicId, setCurTopicId] = useState(current?.topicId);

  const handleTopicChange = (e) => {
    setCurTopicId(e);
    if (current?.topicId && curTopicId != e) {
      form.setFieldsValue({ initPrompt: [] }); // Clear the initPrompt form list
      form.setFieldsValue({ templates: [] }); // Clear the templates form list
    }
  };

  const onSubmit = (fieldsValue) => {
    let dataToUpdate = { ...fieldsValue };
    dispatch(erp.update({ entity, id, jsonData: dataToUpdate }));
  };

  useEffect(() => {
    if (isSuccess) {
      form.resetFields();
      dispatch(erp.resetAction({ actionType: 'update' }));
      navigate(`/${entity.toLowerCase()}/read/${id}`);
    }
  }, [isSuccess]);

  useEffect(() => {
    if (current) {
      setCurrentErp(current);
      let formData = { ...current };
      form.resetFields();
      form.setFieldsValue(formData);
    }
  }, [current]);

  return (
    <>
      <PageHeader
        onBack={() => {
          navigate(`/${entity.toLowerCase()}`);
        }}
        title={translate('update')}
        ghost={false}
        extra={[
          <Button
            key={`${uniqueId()}`}
            onClick={() => {
              navigate(`/${entity.toLowerCase()}`);
            }}
            icon={<CloseCircleOutlined />}
          >
            {translate('Cancel')}
          </Button>,
          <SaveForm translate={translate} form={form} key={`${uniqueId()}`} />,
        ]}
        style={{
          padding: '20px 0px',
        }}
      ></PageHeader>
      <Divider dashed />
      <Loading isLoading={isLoading}>
        <Form form={form} layout="vertical" onFinish={onSubmit}>
          <UpdateForm current={current} handleTopicChange={handleTopicChange} curTopicId={curTopicId} />
        </Form>
      </Loading>
    </>
  );
}
